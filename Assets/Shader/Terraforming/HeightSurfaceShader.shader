Shader "Custom/HeightSurfaceShader" {
	Properties
	{
		texture1("texture1",2D) = "white"{}
		Color1("tint1", Color) = (0,0,0)
		ColorStrength1("color strenght1", Range(0,1)) = 0
		startHeight1("startHeight1", Range(-0.1,1)) = -0.1
		blend1("blend1", Range(0,1)) = 0.1
		textureScale1("scale1",Float) = 1

		texture2("texture2",2D) = "white"{}
		Color2("tint2", Color) = (0,0,0)
		ColorStrength2("color strenght2", Range(0,1)) = 0
		startHeight2("startHeight2", Range(-0.1,1)) = 0.2
		blend2("blend2", Range(0,1)) = 0.1
		textureScale2("scale2",Float) = 1

		texture3("texture3",2D) = "white"{}
		Color3("tint3", Color) = (0,0,0)
		ColorStrength3("color strenght3", Range(0,1)) = 0
		startHeight3("startHeight3", Range(-0.1,1)) = 0.5
		blend3("blend3", Range(0,1)) = 0.1
		textureScale3("scale3",Float) = 1

		texture4("texture4",2D) = "white"{}
		Color4("tint4", Color) = (0,0,0)
		ColorStrength4("color strenght4", Range(0,1)) = 0
		startHeight4("startHeight4", Range(-0.1,1)) = 0.8
		blend4("blend4", Range(0,1)) = 0.1
		textureScale4("scale4",Float) = 1

		minHeight("minHeight", float) = 0
		maxHeight("maxHeight", float) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM

			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			const static float epsilon = 1E-4;

			sampler2D texture1;
			float3 Color1;
			float ColorStrength1;
			float startHeight1;
			float blend1;
			float  textureScale1;

			sampler2D texture2;
			float3 Color2;
			float ColorStrength2;
			float startHeight2;
			float blend2;
			float textureScale2;

			sampler2D texture3;
			float3 Color3;
			float ColorStrength3;
			float startHeight3;
			float blend3;
			float textureScale3;

			sampler2D texture4;
			float3 Color4;
			float ColorStrength4;
			float startHeight4;
			float blend4;
			float textureScale4;

			float minHeight;
			float maxHeight;

			struct Input
			{
				float3 worldPos;
				float3 worldNormal;
			};

			float inverseLerp(float start, float end, float value)
			{
				return saturate((value - start) / (end - start));
			}

			float3 triplanar(float3 worldPos, float scale, float3 blendAxes, sampler2D texuture)
			{
				float3 scaledWorldPos = worldPos / scale;
				float3 xProjection = tex2D(texuture, scaledWorldPos.yz) * blendAxes.x;
				float3 yProjection = tex2D(texuture, scaledWorldPos.xz) * blendAxes.y;
				float3 zProjection = tex2D(texuture, scaledWorldPos.xy) * blendAxes.z;

				return xProjection + yProjection + zProjection;
			}
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
				float3 blendAxes = abs(IN.worldNormal);
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

				float drawStrength = inverseLerp(-blend1 / 2 - epsilon, blend1 / 2,heightPercent - startHeight1);
				float3 baseColor = Color1 * ColorStrength1;
				float3 textureColor = triplanar(IN.worldPos, textureScale1, blendAxes, texture1) * (1 - ColorStrength1);
				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;

				drawStrength = inverseLerp(-blend2 / 2 - epsilon, blend2 / 2, heightPercent - startHeight2);
				baseColor = Color2 * ColorStrength2;
				textureColor = triplanar(IN.worldPos, textureScale2, blendAxes, texture2) * (1 - ColorStrength2);
				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;

				drawStrength = inverseLerp(-blend3 / 2 - epsilon, blend3 / 2, heightPercent - startHeight3);
				baseColor = Color3 * ColorStrength3;
				textureColor = triplanar(IN.worldPos, textureScale3, blendAxes, texture3) * (1 - ColorStrength3);
				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;

				drawStrength = inverseLerp(-blend4 / 2 - epsilon, blend4 / 2, heightPercent - startHeight4);
				baseColor = Color4 * ColorStrength4;
				textureColor = triplanar(IN.worldPos, textureScale4, blendAxes, texture4) * (1 - ColorStrength4);
				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
			}

			ENDCG
		}
			FallBack "Diffuse"
}