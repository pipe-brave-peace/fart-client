using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class SpriteBasicShaderGUI : ShaderGUI
{
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
        Additive,
        Subtractive,
        Modulate,
        Overlay,
        AdditiveOverlay
    }

    public enum CullMode
    {
        Off,
        Front,
        Back
    }

    private static class Styles
    {
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
        public static readonly string[] cullNames = Enum.GetNames(typeof(CullMode));
    }

    private static MaterialProperty blendMode = null;
    private static MaterialProperty cullMode = null;
    private static MaterialProperty zWriteMode = null;
    private static MaterialProperty mainTex = null;
    private static MaterialProperty mainColor = null;

    private static MaterialProperty useCutout = null;
    private static MaterialProperty alphaCutout = null;

    private static MaterialProperty addOutline = null;
    private static MaterialProperty outlineColor = null;
    private static MaterialProperty outlineWidth = null;


    private static MaterialEditor m_MaterialEditor;
    private Material targetMaterial
    {
        get { return (m_MaterialEditor == null) ? null : m_MaterialEditor.target as Material; }
    }

    private bool m_FirstTimeApply = true;

    private static Shader testShader = Shader.Find("Custom/TestShader");
    private static Shader testShader_Outline = Shader.Find("Hidden/Custom/TestShader_Outline");

    public void FindProperties(MaterialProperty[] props)
    {
        blendMode = FindProperty("_Mode", props);
        cullMode = FindProperty("_Cull", props);
        zWriteMode = FindProperty("_ZWrite", props);

        mainTex = FindProperty("_MainTex", props);
        mainColor = FindProperty("_Color", props);

        alphaCutout = FindProperty("_Cutoff", props);
        useCutout = FindProperty("_UseCutout", props);

        addOutline = FindProperty("_AddOutline", props);

        if (addOutline.floatValue > 0)
        {
            try
            {
                outlineColor = FindProperty("_OutlineColor", props, false);
                outlineWidth = FindProperty("_OutlineWidth", props, false);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        else
        {
            outlineColor = null;
            outlineWidth = null;
        }
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        FindProperties(props);

        if (m_FirstTimeApply)
        {
            MaterialChanged(material);
            m_FirstTimeApply = false;
        }

        ShaderPropertiesGUI(material, props);
    }

    public void ShaderPropertiesGUI(Material material, MaterialProperty[] props)
    {
        EditorGUIUtility.labelWidth = 0f;

        EditorGUI.BeginChangeCheck();
        {
            BlendModePopup();

            if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
            {
                m_MaterialEditor.ShaderProperty(
                    alphaCutout,
                    "カットアウトのしきい値",
                    MaterialEditor.kMiniTextureFieldLabelIndentLevel
                );
            }
            else if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Opaque))
            {
                // メニュー追加なし
            }
            else
            {
                m_MaterialEditor.ShaderProperty(zWriteMode, "ZWrite OnOff");
            }

            CullModePopup();

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2.0f) });

            m_MaterialEditor.TextureProperty(mainTex, "Texture");
            m_MaterialEditor.ColorProperty(mainColor, "Color");

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2.0f) });

            m_MaterialEditor.ShaderProperty(addOutline, "Add Outline");

            if (addOutline.floatValue > 0)
            {
                m_MaterialEditor.SetShader(testShader_Outline);

                FindProperties(props);

                if (outlineColor != null)
                {
                    m_MaterialEditor.ColorProperty(outlineColor, "Outline Color");
                }

                if (outlineWidth != null)
                {
                    m_MaterialEditor.RangeProperty(outlineWidth, "Outline Width");
                }
            }
            else
            {
                m_MaterialEditor.SetShader(testShader);
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            // CutoutのONOFF ===================================================
            if ((BlendMode)material.GetFloat("_Mode") == BlendMode.Opaque)
            {
                zWriteMode.floatValue = 1.0f;
                useCutout.floatValue = 0;
                material.DisableKeyword("_USECUTOUT_ON");
            }
            else if ((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout)
            {
                zWriteMode.floatValue = 1.0f;
                useCutout.floatValue = 1.0f;
                material.EnableKeyword("_USECUTOUT_ON");
            }
            else
            {
                useCutout.floatValue = 0;
                material.DisableKeyword("_USECUTOUT_ON");
            }

            m_MaterialEditor.PropertiesChanged();

            if (m_MaterialEditor.targets != null && m_MaterialEditor.targets.Length > 0)
            {
                foreach (UnityEngine.Object t in m_MaterialEditor.targets)
                {
                    EditorUtility.SetDirty(t);
                }
            }
            else
            {
                EditorUtility.SetDirty(targetMaterial);
            }
        }
    }

    static void MaterialChanged(Material material)
    {
        SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (oldShader != newShader)
        {
            if (oldShader == null)
            {
                SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
                return;
            }
            if (oldShader.name == "Custom/TestShader" || oldShader.name == "Hidden/Custom/testShader_Outline")
            {
                return;
            }
        }
        MaterialChanged(material);
    }

    void BlendModePopup()
    {
        EditorGUI.showMixedValue = blendMode.hasMixedValue;
        var mode = (BlendMode)blendMode.floatValue;

        var dispNames = ChangeDisplayBlendName(Styles.blendNames);

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup("Rendering Mode", (int)mode, dispNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
            blendMode.floatValue = (float)mode;
        }
        EditorGUI.showMixedValue = false;
    }

    void CullModePopup()
    {
        EditorGUI.showMixedValue = cullMode.hasMixedValue;
        var mode = (CullMode)cullMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (CullMode)EditorGUILayout.Popup("Culling", (int)mode, Styles.cullNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Culling Mode");
            cullMode.floatValue = (float)mode;
        }
        EditorGUI.showMixedValue = false;
    }

    private static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "Opaque");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Additive:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Subtractive:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Modulate:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Overlay:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.AdditiveOverlay:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }

    private static string[] ChangeDisplayBlendName(string[] blendNames)
    {
        string[] tempNameArray = new string[blendNames.Length];

        for (var i = 0; i < blendNames.Length; i++)
        {
            if (blendNames[i] == "Opaque")
            {
                tempNameArray[i] = "Opaque (不透明)";
            }
            else if (blendNames[i] == "Cutout")
            {
                tempNameArray[i] = "Cutout (2値化抜き)";
            }
            else if (blendNames[i] == "Fade")
            {
                tempNameArray[i] = "Fade (透明 通常)";
            }
            else if (blendNames[i] == "Transparent")
            {
                tempNameArray[i] = "Transparent (透明 プラスチック・ガラス向き)";
            }
            else if (blendNames[i] == "Additive")
            {
                tempNameArray[i] = "Additive (加算)";
            }
            else if (blendNames[i] == "Subtractive")
            {
                tempNameArray[i] = "Subtractive (減算)";
            }
            else if (blendNames[i] == "Modulate")
            {
                tempNameArray[i] = "Modulate (乗算)";
            }
            else if (blendNames[i] == "Overlay")
            {
                tempNameArray[i] = "Overlay (オーバーレイ)";
            }
            else if (blendNames[i] == "AdditiveOverlay")
            {
                tempNameArray[i] = "AdditiveOverlay (加算オーバーレイ)";
            }
        }
        return tempNameArray;
    }
}