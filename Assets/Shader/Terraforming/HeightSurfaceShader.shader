Shader "Custom/HeightSurfaceShader" {
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static float epsilon = 1E-4;
		const static int maxLayerCount = 8;

		int layerCount;

		float3 baseColours[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColourStrength[maxLayerCount];
		float baseTextureScales[maxLayerCount];

		UNITY_DECLARE_TEX2DARRAY(baseTextures);

		float minHeight;
		float maxHeight;

		// Fog of war
		float4 FogColor;
		float2 BoundCenter;
		float Radius;

		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		float inverseLerp(float start, float end, float value)
		{
			return saturate((value - start) / (end - start));
		}

		float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
		{
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		}
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			for (int i = 0; i < layerCount; i++)
			{
				float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

				float3 baseColour = baseColours[i] * baseColourStrength[i];
				float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColourStrength[i]);

				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
			}

			//apply fog
			o.Albedo *= saturate(FogColor + (smoothstep(0, .4, max(Radius - length(IN.worldPos.xz - BoundCenter), 0) / Radius)));
		}

		ENDCG
	}
		FallBack "Diffuse"
}