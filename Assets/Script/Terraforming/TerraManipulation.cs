using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainData))]
public class TerraManipulation : MonoBehaviour
{
    public LayerMask TerrainLayer;
    public Camera MainCamera;

    public int PullUpFactor;
    public int PushDownFactor;

    public Brush brush;

    private Terrain Terra;
    private TerrainData TData;

    private int mouseImpactX;
    private int mouseImpactY;

    // Use this for initialization
    private void Start()
    {
        Terra = GetComponent<Terrain>();
        TData = Terra.terrainData;
        ResetTerrain();
    }

    private void ChangeTerrain(int basisX, int basisY, int value)
    {
        int width = brush.BrushWidth;
        int height = brush.BrushHeight;

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

        TData.SetHeights(basisX, basisY, brush.CalculateBrushValues(TData.GetHeights(basisX, basisY, width, height), value));
    }

    private bool CalculateInpactPoint()
    {
        var mousePos = Input.mousePosition;
        mouseImpactX = 0;
        mouseImpactY = 0;
        brush.HoverIndicator.SetActive(false);
        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(mousePos);
        bool hasHit;
        if (hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, TerrainLayer))
        {
            Vector3 relativePoint = (hit.point - transform.position);

            brush.HoverIndicator.SetActive(true);
            brush.HoverIndicator.transform.position = relativePoint;

            Vector3 normalizedTerrainCoords = new Vector3(relativePoint.x / TData.size.x, relativePoint.y / TData.size.y, relativePoint.z / TData.size.z);
            int TerrainSpaceX = (int)(normalizedTerrainCoords.x * TData.heightmapWidth);
            int TerrainSpaceY = (int)(normalizedTerrainCoords.z * TData.heightmapHeight);
            mouseImpactX = TerrainSpaceX - (int)(brush.BrushWidth * 0.5f);
            mouseImpactY = TerrainSpaceY - (int)(brush.BrushHeight * 0.5f);
        }
        return hasHit;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (CalculateInpactPoint())
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                ChangeTerrain(mouseImpactX, mouseImpactY, PullUpFactor);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                ChangeTerrain(mouseImpactX, mouseImpactY, PushDownFactor);
            }
        }
    }

    private void ResetTerrain()
    {
        float[,] tempHeight = new float[TData.heightmapHeight, TData.heightmapWidth];
        TData.SetHeights(0, 0, tempHeight);
    }
}