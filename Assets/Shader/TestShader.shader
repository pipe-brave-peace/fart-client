// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TestShader"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        [KeywordEnum(Off, Front, Back)] _Cull ("Culling", Float) = 2 

        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [Toggle] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector][Toggle] _UseCutout("__useCutout", Float) = 0

        [Toggle] _AddOutline("__addOutline", Float) = 0
    }

     SubShader
    {
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            Cull [_Cull]
            Lighting Off
            ZWrite [_ZWrite]

            Name "Main"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _ _USECUTOUT_ON

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            #ifdef _USECUTOUT_ON
                fixed _Cutoff;
            #endif

            struct appdata
            {
                float4 position : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.position = UnityObjectToClipPos(v.position);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;

                #ifdef _USECUTOUT_ON
                    clip( col.a - _Cutoff);
                #endif

                return col;
            }
            ENDCG
        }
    }
    CustomEditor "TestShaderGUI"
}
