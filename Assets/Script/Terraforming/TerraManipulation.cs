using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainData))]
public class TerraManipulation : MonoBehaviour
{
    public LayerMask TerrainLayer;
    public Camera MainCamera;

    private Terrain Terra;
    private TerrainData TData;

    // Use this for initialization
    private void Start()
    {
        Terra = GetComponent<Terrain>();
        TData = Terra.terrainData;
        ResetTerrain();
    }

    public void ChangeTerrain(int basisX, int basisY, int width, int height, Func<float[,], float[,]> brushFunction)
    {
        //Bound check
        if (basisX < 0)
        {
            width += basisX;
            basisX = 0;
        }

        if (basisY < 0)
        {
            height += basisY;
            basisY = 0;
        }

        if ((basisX + width) > TData.heightmapWidth)
        {
            width = width - ((basisX + width) - TData.heightmapWidth);
        }

        if ((basisY + height) > TData.heightmapHeight)
        {
            height = height - ((basisY + height) - TData.heightmapHeight);
        }

        TData.SetHeights(basisX, basisY, brushFunction(TData.GetHeights(basisX, basisY, width, height)));
    }

    public bool CalculateInpactPoint(Vector3 mousePos, out Vector3 relativePoint, out Vector2 mouseImpact)
    {
        mouseImpact = new Vector2();
        relativePoint = new Vector3();
        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(mousePos);
        bool hasHit;
        if (hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, TerrainLayer))
        {
            relativePoint = (hit.point - transform.position);
            Vector3 normalizedTerrainCoords = new Vector3(relativePoint.x / TData.size.x, relativePoint.y / TData.size.y, relativePoint.z / TData.size.z);
            int TerrainSpaceX = (int)(normalizedTerrainCoords.x * TData.heightmapWidth);
            int TerrainSpaceY = (int)(normalizedTerrainCoords.z * TData.heightmapHeight);
            mouseImpact.x = TerrainSpaceX;
            mouseImpact.y = TerrainSpaceY;
        }
        return hasHit;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
    }

    private void ResetTerrain()
    {
        float[,] tempHeight = new float[TData.heightmapHeight, TData.heightmapWidth];
        TData.SetHeights(0, 0, tempHeight);
    }
}