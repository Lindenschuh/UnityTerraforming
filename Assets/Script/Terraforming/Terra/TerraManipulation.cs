using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainData))]
public class TerraManipulation : Photon.PunBehaviour
{
    public LayerMask TerrainLayer;
    public Camera MainCamera;
    public GodStateManager GodState;
    public GodData GD;
    public GodControlls GodCon;

    private Terrain Terra;
    private TerrainData TData;

    private Vector3 lastRelative;
    private Vector2Int lastImpact;

    // Use this for initialization
    private void Start()
    {
        Terra = GetComponent<Terrain>();
        TData = Terra.terrainData;
        //ResetTerrain();
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

    public bool isInRange()
    {
        return (lastRelative.xz() - GodState.BoundCenter.position.xz()).magnitude <= GodState.BoundRadius;
    }

    private void FixedUpdate()
    {
        if (GodState.BrushMng.gameObject.GetActive())
            BrushBehaivor();
        else
            AbilityBehaivor();
    }

    private void AbilityBehaivor()
    {
        if (CalculateInpactPoint(Input.mousePosition, new Vector2Int((int)GodState.AbilityMng.CurrentAbility.Radius * 2, (int)GodState.AbilityMng.CurrentAbility.Radius * 2), out lastRelative, out lastImpact))
        {
            if (isInRange())
            {
                GodState.AbilityMng.CurrentAbility.PlaceIndicator(lastRelative);
                if (GodCon.Lift)
                {
                    photonView.RPC("RPCUseAbility", PhotonTargets.All, lastRelative);
                }
            }
        }
    }

    private void BrushBehaivor()
    {
        if (CalculateInpactPoint(Input.mousePosition, new Vector2Int(GodState.BrushMng.CurrentActive.BrushWidth, GodState.BrushMng.CurrentActive.BrushHeight), out lastRelative, out lastImpact))
        {
            if (isInRange())
            {
                GodState.BrushMng.CurrentActive.PlaceIndicatorPositive(lastRelative);
                if (GodCon.Lift)
                {
                    if (GD.Lift())
                        photonView.RPC("RPCLiftTerrain", PhotonTargets.All, lastImpact.x, lastImpact.y, GodState.BrushMng.CurrentActive.BrushWidth, GodState.BrushMng.CurrentActive.BrushHeight);
                }
                if (GodCon.Lower)
                {
                    if (GD.Lower())
                        photonView.RPC("RPCLowerTerrain", PhotonTargets.All, lastImpact.x, lastImpact.y, GodState.BrushMng.CurrentActive.BrushWidth, GodState.BrushMng.CurrentActive.BrushHeight);
                }
            }
            else
            {
                GodState.BrushMng.CurrentActive.PlaceIndicatorNegative(lastRelative);
            }
        }
    }

    public bool CalculateInpactPoint(Vector3 mousePos, Vector2Int size, out Vector3 relativePoint, out Vector2Int mouseImpact)
    {
        mouseImpact = new Vector2Int();
        relativePoint = new Vector3();
        if (GodCon == null || !GodCon.isActiveAndEnabled)
            return false;

        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(mousePos);
        bool hasHit;
        if (hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, TerrainLayer))
        {
            relativePoint = (hit.point - transform.position);
            Vector3 normalizedTerrainCoords = new Vector3(relativePoint.x / TData.size.x, relativePoint.y / TData.size.y, relativePoint.z / TData.size.z);
            int TerrainSpaceX = (int)(normalizedTerrainCoords.x * TData.heightmapWidth);
            int TerrainSpaceY = (int)(normalizedTerrainCoords.z * TData.heightmapHeight);
            mouseImpact.x = TerrainSpaceX - size.x / 2;
            mouseImpact.y = TerrainSpaceY - size.y / 2;
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
    private void RPCUseAbility(Vector3 position)
    {
        GodState.AbilityMng.CurrentAbility.UseAbility(position);
    }

    [PunRPC]
    private void RPCLiftTerrain(int basisX, int basisY, int width, int height)
    {
        //Bound check
        CheckBounds(ref basisX, ref basisY, ref width, ref height);
        TData.SetHeights(basisX, basisY, GodState.BrushMng.CurrentActive.CalculateBrushUp(TData.GetHeights(basisX, basisY, width, height)));
    }

    [PunRPC]
    private void RPCLowerTerrain(int basisX, int basisY, int width, int height)
    {
        //Bound check
        CheckBounds(ref basisX, ref basisY, ref width, ref height);
        TData.SetHeights(basisX, basisY, GodState.BrushMng.CurrentActive.CalculateBrushDown(TData.GetHeights(basisX, basisY, width, height)));
    }

    #endregion PunRPC
}