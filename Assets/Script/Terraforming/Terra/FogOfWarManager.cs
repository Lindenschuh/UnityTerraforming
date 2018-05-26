using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FogOfWarManager : MonoBehaviour
{
    private const int textureSize = 512;
    private const TextureFormat textureFormat = TextureFormat.RGB565;

    // Layer

    public float MinHeight = 0f;
    public float MaxHeight = 100f;
    public Layer[] Layers;

    //FOG Paras

    public BrushSwitch BS;
    public Color FogColor;
    public float FogOfWarUpdateSpeed;

    //Private

    private Terrain tRain;
    private float CurrentRadius;
    private Texture2DArray texturesArray;

    private void Start()
    {
        CurrentRadius = BS.BoundRadius;
        tRain = GetComponent<Terrain>();
        texturesArray = GenerateTextureArray(Layers.Select(x => x.texture).ToArray());
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentRadius = Mathf.Lerp(CurrentRadius, BS.BoundRadius, FogOfWarUpdateSpeed * Time.deltaTime);

        MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();

        materialProperty.SetFloat("layerCount", Layers.Length);
        materialProperty.SetVectorArray("baseColors", Layers.Select(x => new Vector4(x.tint.r, x.tint.g, x.tint.b, x.tint.a)).ToArray());
        materialProperty.SetFloatArray("baseStartHeights", Layers.Select(x => x.startHeight).ToArray());
        materialProperty.SetFloatArray("baseBlends", Layers.Select(x => x.blendStrength).ToArray());
        materialProperty.SetFloatArray("baseColourStrength", Layers.Select(x => x.tintStrength).ToArray());
        materialProperty.SetFloatArray("baseTextureScales", Layers.Select(x => x.textureScale).ToArray());

        materialProperty.SetTexture("baseTextures", texturesArray);
        //Fog
        materialProperty.SetFloat("Radius", CurrentRadius);
        materialProperty.SetVector("BoundCenter", new Vector2(BS.BoundCenter.position.x, BS.BoundCenter.position.z));
        materialProperty.SetColor("FogColor", FogColor);

        materialProperty.SetFloat("minHeight", MinHeight);
        materialProperty.SetFloat("maxHeight", MaxHeight);

        tRain.SetSplatMaterialPropertyBlock(materialProperty);
    }

    private Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;

        [Range(0, 1)]
        public float tintStrength;

        [Range(-0.01f, 1)]
        public float startHeight;

        [Range(0, 1)]
        public float blendStrength;

        public float textureScale;
    }
}