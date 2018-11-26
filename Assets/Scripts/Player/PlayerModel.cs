using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour {

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    Transform m_Rotation;

    public bool m_bStart;

    Color m_FadeColor;

    [SerializeField]
    Material material;

    // Use this for initialization
    void Start () {
        m_FadeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);      // 現在の色をセット
        material.SetColor("_MainColor", m_FadeColor);
    }
	
	// Update is called once per frame
	void Update () {

        if (m_bStart)
        {
            m_Rotation.localEulerAngles = Vector3.Lerp(m_Rotation.localEulerAngles, new Vector3(0, 0, 0), 0.1f);
            m_Animator.SetBool("StartBool", true);
        }
    }

    public void FadeInModel(float value)
    {
        if (m_FadeColor.a < 1)
        {
            m_FadeColor.a += value;
            material.SetColor("_MainColor", m_FadeColor);
        }
        else if (m_FadeColor.a >= 1)
        {
            m_FadeColor.a = 1;
        }
    }

    public void FadeOutModel(float value)
    {
        if (m_FadeColor.a > 0)
        {
            m_FadeColor.a -= value;
            material.SetColor("_MainColor", m_FadeColor);
        }
        else if (m_FadeColor.a < 0)
        {
            m_FadeColor.a = 0;
        }
    }
}
