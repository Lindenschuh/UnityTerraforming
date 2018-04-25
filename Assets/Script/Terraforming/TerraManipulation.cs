using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainData))]
public class TerraManipulation : Photon.PunBehaviour
{
    public LayerMask TerrainLayer;
    public Camera MainCamera;
    public BrushSwitch BrushSwitcher;

    private Terrain Terra;
    private TerrainData TData;

    private Vector3 lastRelative;
    private Vector2Int lastImpact;

    // Use this for initialization
    private void Start()
    {
        Terra = GetComponent<Terrain>();
        TData = Terra.terrainData;
        ResetTerrain();
    }

    private void CheckBounds(ref int basisX, ref int basisY, ref int width, ref int height)
    {
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
    }

    private void FixedUpdate()
    {
        if (CalculateInpactPoint(Input.mousePosition, out lastRelative, out lastImpact))
        {
            if (BrushSwitcher.CurrentActive.IsAreaFree(lastRelative))
            {
                if (Input.GetMouseButton(0))
                {
                    //LiftTerrain(lastImpact.x, lastImpact.y, BrushSwitcher.CurrentActive.BrushWidth, BrushSwitcher.CurrentActive.BrushHeight);
                    photonView.RPC("RPCLiftTerrain", PhotonTargets.All, lastImpact.x, lastImpact.y, BrushSwitcher.CurrentActive.BrushWidth, BrushSwitcher.CurrentActive.BrushHeight);
                }
                if (Input.GetMouseButton(1))
                {
                    //LowerTerrain(lastImpact.x, lastImpact.y, BrushSwitcher.CurrentActive.BrushWidth, BrushSwitcher.CurrentActive.BrushHeight);12
                    photonView.RPC("RPCLowerTerrain", PhotonTargets.All, lastImpact.x, lastImpact.y, BrushSwitcher.CurrentActive.BrushWidth, BrushSwitcher.CurrentActive.BrushHeight);
                }
            }
        }
    }

    public bool CalculateInpactPoint(Vector3 mousePos, out Vector3 relativePoint, out Vector2Int mouseImpact)
    {
        mouseImpact = new Vector2Int();
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
            mouseImpact.x = TerrainSpaceX - BrushSwitcher.CurrentActive.BrushWidth / 2;
            mouseImpact.y = TerrainSpaceY - BrushSwitcher.CurrentActive.BrushHeight / 2;
        }
        return hasHit;
    }

    private void ResetTerrain()
    {
        float[,] tempHeight = new float[TData.heightmapHeight, TData.heightmapWidth];
        TData.SetHeights(0, 0, tempHeight);
    }

    #region PunRPC

    [PunRPC]
    private void RPCLiftTerrain(int basisX, int basisY, int width, int height)
    {
        //Bound check
        CheckBounds(ref basisX, ref basisY, ref width, ref height);
        TData.SetHeights(basisX, basisY, BrushSwitcher.CurrentActive.CalculateBrushUp(TData.GetHeights(basisX, basisY, width, height)));
    }

    [PunRPC]
    private void RPCLowerTerrain(int basisX, int basisY, int width, int height)
    {
        //Bound check
        CheckBounds(ref basisX, ref basisY, ref width, ref height);
        TData.SetHeights(basisX, basisY, BrushSwitcher.CurrentActive.CalculateBrushUp(TData.GetHeights(basisX, basisY, width, height)));
    }

    #endregion PunRPC
}