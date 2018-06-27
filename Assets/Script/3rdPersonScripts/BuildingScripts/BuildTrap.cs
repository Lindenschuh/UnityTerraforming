using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildTrap : Photon.PunBehaviour {

    private enum TrapType {TrapFire, TrapSpike, None }

    public Material[] TrapMaterials;
    public LayerMask BuildingLayers;
    public float maxBuildingRange;

    private TrapType actualTrapType;
    private bool buildAllowed;
    private bool trapBuildingActive;
    private Camera playerCam;
    private Transform actualTransform;
    private Color actualTransformColor;
    private ResourceControl resourceControl;
    private InventoryManager inventoryManager;
    private bool firstVisit;
    private Vector3 normalVector;
    private Invector.vShooter.vShooterMeleeInput shooterInput;
    // Use this for initialization
    void Start ()
    {
        if (maxBuildingRange < 1)
            maxBuildingRange = 1;
        playerCam = Camera.main;
        buildAllowed = false;
        resourceControl = GetComponent<ResourceControl>();
        actualTrapType = TrapType.None;
        trapBuildingActive = false;
        firstVisit = true;
        shooterInput = gameObject.GetComponent<Invector.vShooter.vShooterMeleeInput>();
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
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    photonView.RPC("RPCSetTrap", PhotonTargets.All, actualTrapType.ToString(), actualTransform.position, normalVector);
                    GameObject.Find("UI").GetComponentInChildren<InventoryManager>(true).RemoveTrap();
                    ResetBuilding();
                }
            }
        }
    }

    private void GetTrapOfKey()
    {       
        if (Input.GetKeyUp(KeyCode.F5))
        {
            if (trapBuildingActive)
            {
                trapBuildingActive = false;
                actualTrapType = TrapType.None;
                firstVisit = true;
                shooterInput.lockMeleeInput = false;
                shooterInput.lockShooterInput = false;
            }
            else
            {
                trapBuildingActive = true;
                shooterInput.lockMeleeInput = true;
                shooterInput.lockShooterInput = true;
            }
        }

        if (trapBuildingActive)
            InitializeTrapBuild(resourceControl.GetSelectedTrap());

        if (trapBuildingActive && Input.GetKeyUp(KeyCode.Mouse1))
        {
            resourceControl.SelectNextTrap();
        }
    }

    private void InitializeTrapBuild(GameObject trapPrefab)
    {
        if (trapPrefab == null || (actualTrapType != TrapType.None && actualTrapType.ToString() == trapPrefab.name))
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
            // check for color of first visited prefab
            if (firstVisit)
            {
                actualTransformColor = hit.transform.GetComponent<Renderer>().material.color;
                firstVisit = false;
            }

            // check if new building has been selected
            if (actualTransform != null && actualTransform != hit.transform)
            {
                ResetBuilding();
                actualTransformColor = hit.transform.GetComponent<Renderer>().material.color;
            }

            actualTransform = hit.transform;

            // check if a trap has already been activated
            if (hit.transform.childCount <= 0)
            {
                hit.transform.GetComponent<Renderer>().material.color = Color.green;
                buildAllowed = true;
            }
            else
            {
                hit.transform.GetComponent<Renderer>().material.color = Color.red;
                buildAllowed = false;
            }
            normalVector = hit.normal;
        }
        else
        {
            if (actualTransform != null)
            {
                ResetBuilding();
                buildAllowed = false;
            }
        }
    }

    private void ResetBuilding()
    {
        actualTransform.GetComponent<Renderer>().material.color = actualTransformColor;
        actualTransform = null;
    }

    [PunRPC]
    private void RPCSetTrap(string prefabName, Vector3 position, Vector3 normalVector)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.001f);
        if (hitColliders.Length > 0)
        {
            GameObject tempRpcTrap = Resources.Load(prefabName) as GameObject;
            GameObject rpcTrap = Instantiate(tempRpcTrap);
            rpcTrap.GetComponent<Trap>().enabled = true;
            rpcTrap.transform.parent = hitColliders[0].transform;
            rpcTrap.transform.position = hitColliders[0].transform.position + (0.05f * normalVector);
            rpcTrap.transform.rotation = hitColliders[0].transform.rotation;
            rpcTrap.transform.localScale = new Vector3(1, 1, 1);
            rpcTrap.GetComponent<Trap>().direction = normalVector;
        }
    }
}
