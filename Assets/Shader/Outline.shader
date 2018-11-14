Shader "Custom/OutlineSurfaceShader" {

	Properties{
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 0)
		_OutlineWidth("Outline Width", float) = 0.1
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}

		SubShader{
			Tags{ "Queue" = "Transparent" }

		//1パス目.

		Cull Front

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert alpha:fade

		float4 _MainColor;
		float4 _OutlineColor;
		float _OutlineWidth;

		struct Input {
			float4 vertexColor : COLOR;
		};

		void vert(inout appdata_full v, out Input o) {
			float distance = -UnityObjectToViewPos(v.vertex).z;
			v.vertex.xyz += v.normal * distance * _OutlineWidth;
			o.vertexColor = v.color;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _OutlineColor.rgb;
			o.Alpha = 1;
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG


		//2パス目.

		Cull Back

		CGPROGRAM

		#pragma surface surf Lambert alpha:fade

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		float4 _MainColor;

		sampler2D _MainTex;

		sampler2D _BumpMap;

		float4 _RimColor;

		float _RimPower;

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgba * _MainColor;
			o.Alpha = 1;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
		Fallback "Diffuse"
}