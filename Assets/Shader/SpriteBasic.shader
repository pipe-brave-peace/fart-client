// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteBasicShaderGUI"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
	_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		[KeywordEnum(Off, Front, Back)] _Cull("Culling", Float) = 2

		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[Toggle] _ZWrite("__zw", Float) = 1.0
		[HideInInspector][Toggle] _UseCutout("__useCutout", Float) = 0

		[Toggle] _AddOutline("__addOutline", Float) = 0
		_OutlineColor("Outline Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_OutlineWidth("Outline Width", Range(0.0, 1.0)) = 0.1
	}

		SubShader
	{
		UsePass "Custom/TestShader/MAIN"

		//Outline パス
		Pass
	{
		Name "Outline"
		Cull Front
		Blend[_SrcBlend][_DstBlend]
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 position : POSITION;
		float3 normal   : NORMAL;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
	};

	fixed4 _OutlineColor;
	fixed _OutlineWidth;

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);

		o.position = UnityObjectToClipPos(v.position);

		float3 nml = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
		float2 offset = TransformViewToProjection(nml.xy);

		o.position.xy += offset * o.position.z * _OutlineWidth / 10;

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return _OutlineColor;
	}
		ENDCG
	}
	}
		CustomEditor "SpriteBasicShaderGUI.cs"
}