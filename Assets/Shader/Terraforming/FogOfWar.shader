Shader "Custom/FogOfWar" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			LOD 200

			CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float3 worldPos;
			float2 uv_MainTex;
		};

		float4 _Color;
		float2 _BoundCenter;
		float _Radius;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = _Color;

			o.Alpha = c.a;
			//1.0 - saturate(sign(_Radius - length(IN.worldPos.xz - _BoundCenter)));

		o.Albedo = c.rgb;
	}
	ENDCG
	}
		FallBack "Diffuse"
}