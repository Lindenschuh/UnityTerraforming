﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildTrap : Photon.PunBehaviour {

    private enum TrapType {TrapFire, TrapIce, None }

    public Material[] TrapMaterials;
    public LayerMask BuildingLayers;
    public float maxBuildingRange;
    public int materialCost;

    private TrapType actualTrapType;
    private bool buildAllowed;
    private bool trapBuildingActive;
    private Camera playerCam;
    private Transform actualTransform;
    private Color actualTransformColor;
    private ResourceControl resourceControl;
    private BuildResources selectedMaterial;
    private InventoryManager inventoryManager;
    // Use this for initialization
    void Start ()
    {
        if (maxBuildingRange < 1)
            maxBuildingRange = 1;
        playerCam = Camera.main;
        buildAllowed = false;
        resourceControl = GetComponent<ResourceControl>();
        selectedMaterial = BuildResources.TrapInstant;
        trapBuildingActive = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        GetTrapOfKey();

        if (actualTrapType != TrapType.None)
        {
            CheckForBuilding();
            if (buildAllowed)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0) && resourceControl.GetResourceInfo(selectedMaterial) >= materialCost)
                {
                    photonView.RPC("RPCSetTrap", PhotonTargets.All, actualTrapType.ToString(), Input.mousePosition, maxBuildingRange, BuildingLayers);
                    resourceControl.UseResource(selectedMaterial, materialCost);
                }
            }
        }
    }

    private void GetTrapOfKey()
    {
        
        if (Input.GetKeyUp(KeyCode.F5))
        {
            if (trapBuildingActive) trapBuildingActive = false;
            else trapBuildingActive = true;
            if(trapBuildingActive) InitializeTrapBuild(resourceControl.GetSelectedTrap());
        }
        if (trapBuildingActive && Input.GetKeyUp(KeyCode.Mouse1))
        {
            resourceControl.SelectNextTrap();
        }
    }

    private void InitializeTrapBuild(GameObject trapPrefab)
    {
        if (actualTrapType != TrapType.None && actualTrapType.ToString() == trapPrefab.name)
        {
            actualTrapType = TrapType.None;
        }
        else
        {
            actualTrapType = (TrapType) Enum.Parse(typeof(TrapType), trapPrefab.name);
        }
    }

    private void CheckForBuilding()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, maxBuildingRange, BuildingLayers))
        {
            // check if new building has been selected
            if (actualTransform != hit.transform)
            {
                ResetBuilding();
                actualTransform = hit.transform;
                actualTransformColor = hit.transform.GetComponent<Renderer>().material.color;
            }

            // check if a trap has already been activated
            if (!hit.transform.GetComponent<Trap>().enabled)
            {
                hit.transform.GetComponent<Renderer>().material.color = Color.green;
                buildAllowed = true;
            }
            else
            {
                hit.transform.GetComponent<Renderer>().material.color = Color.red;
                buildAllowed = false;
            }
        }
        else
        {
            ResetBuilding();
            buildAllowed = false;
        }
    }

    private void ResetBuilding()
    {
        actualTransform.GetComponent<Renderer>().material.color = actualTransformColor;
        actualTransform = null;
    }

    [PunRPC]
    private void RPCSetTrap(string prefabName, Vector3 mousePosition, float maxBuildingRange, LayerMask BuildingLayers)
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, maxBuildingRange, BuildingLayers))
        {
            GameObject rpcTrap = Resources.Load(prefabName) as GameObject;
            rpcTrap.GetComponent<Trap>().enabled = true;
            rpcTrap.transform.parent = hit.transform;
        }
    }
}
