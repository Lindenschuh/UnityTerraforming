Shader "Custom/FogOfWar" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

			LOD 200

			CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:auto

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

			o.Alpha = 1.0 - smoothstep(0,.4,max(_Radius - length(IN.worldPos.xz - _BoundCenter),0) / _Radius);
			o.Albedo = c.rgb;
		}

	ENDCG
	}
		FallBack "Diffuse"
}