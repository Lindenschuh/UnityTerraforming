using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Photon.PunBehaviour {

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

    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (PhotonNetwork.isMasterClient)
        {
            seed = Random.Range(0, 100000);
            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for (int i = 0; i < octaves; i++)
            {
                float offSetX = prng.Next(-100000, 100000) + offset.x;
                float offSetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offSetX, offSetY);
            }
            photonView.RPC("RPCGenerateMap", PhotonTargets.AllBufferedViaServer, octaveOffsets);
        }      
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }

    [PunRPC]
    private void RPCGenerateMap(Vector2[] octaveOffsets)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, octaveOffsets, noiseScale, octaves, persistance, lacunarity, offset);
        TerrainData terrain = GameObject.Find("Terrain").GetComponent<Terrain>().terrainData;
        terrain.SetHeights(0, 0, noiseMap);
    }
}
