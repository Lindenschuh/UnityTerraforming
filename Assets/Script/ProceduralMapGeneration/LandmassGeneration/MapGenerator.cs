﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public int width = 513;
    public int height = 513;

    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public float[,] GenerateMap (int pSeed = 0)
    {
        if (pSeed != 0)
            seed = Random.Range(0, 100000);
        else
            seed = pSeed;
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);
        TerrainData terrain = GameObject.Find("Terrain").GetComponent<Terrain>().terrainData;
        terrain.SetHeights(0, 0, noiseMap);
        return noiseMap;
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}