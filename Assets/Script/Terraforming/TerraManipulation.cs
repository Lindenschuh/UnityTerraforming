using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainData))]
public class TerraManipulation : MonoBehaviour
{
    public LayerMask TerrainLayer;
    public Camera MainCamera;

    private GameObject HoverIndicator;

    public int BrushWidth;
    public int BrushHeight;

    public float PullUpFactor;
    public float PushDownFactor;

    public Material IndicatorMaterial;

    private Terrain Terra;
    private TerrainData TData;
    private TerrainCollider TColl;

    private int mouseImpactX;
    private int mouseImpactY;

    // Use this for initialization
    private void Start()
    {
        Terra = GetComponent<Terrain>();
        TData = Terra.terrainData;
        TColl = GetComponent<TerrainCollider>();
        HoverIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        HoverIndicator.transform.localScale = new Vector3(BrushWidth, .5f, BrushHeight);
        HoverIndicator.SetActive(false);
        HoverIndicator.GetComponent<Renderer>().material = IndicatorMaterial;
        ResetTerrain();
    }

    private void ChangeTerrain(int basisX, int basisY, int width, int height, float value)
    {
        float[,] tempHeight = TData.GetHeights(basisX, basisY, width, height);
        for (int y = 0; y < tempHeight.GetLength(0); y++)
        {
            for (int x = 0; x < tempHeight.GetLength(1); x++)
            {
                tempHeight[y, x] += value;
            }
        }

        TData.SetHeights(basisX, basisY, tempHeight);
    }

    private bool CalculateInpactPoint()
    {
        var mousePos = Input.mousePosition;
        mouseImpactX = 0;
        mouseImpactY = 0;
        HoverIndicator.SetActive(false);
        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(mousePos);
        bool hasHit;
        if (hasHit = Physics.Raycast(ray, out hit, TerrainLayer))
        {
            Vector3 relativePoint = (hit.point - transform.position);

            HoverIndicator.SetActive(true);
            HoverIndicator.transform.position = relativePoint;

            Vector3 normalizedTerrainCoords = new Vector3(relativePoint.x / TData.size.x, relativePoint.y / TData.size.y, relativePoint.z / TData.size.z);
            int TerrainSpaceX = (int)(normalizedTerrainCoords.x * TData.heightmapWidth);
            int TerrainSpaceY = (int)(normalizedTerrainCoords.z * TData.heightmapHeight);
            mouseImpactX = TerrainSpaceX - (int)(BrushWidth * 0.5f);
            mouseImpactY = TerrainSpaceY - (int)(BrushHeight * 0.5f);
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
                ChangeTerrain(mouseImpactX, mouseImpactY, BrushWidth, BrushHeight, PullUpFactor);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                ChangeTerrain(mouseImpactX, mouseImpactY, BrushWidth, BrushHeight, PushDownFactor);
            }
        }
    }

    private void ResetTerrain()
    {
        float[,] tempHeight = new float[TData.heightmapHeight, TData.heightmapWidth];
        TData.SetHeights(0, 0, tempHeight);
    }
}