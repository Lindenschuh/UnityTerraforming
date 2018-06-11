using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : Photon.PunBehaviour {

    public GameObject[] TrapPrefabs;
    public KeyCode[] TrapKeyCodes;
    public LayerMask BuildingLayers;
    public float maxBuildingRange;
    public int materialCost;

    private bool buildAllowed;
    private bool trapBuildingActive;
    private Camera playerCam;
    private GameObject actualTrap;
    private GameObject trap;
    private Transform actualHit;
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

        if (actualTrap != null)
        {
            CheckForBuilding();
            if (buildAllowed)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0) && resourceControl.GetResourceInfo(selectedMaterial) >= materialCost)
                {
                    photonView.RPC("RPCSetTrap", PhotonTargets.All, actualTrap.name, trap.transform.position, trap.transform.rotation, trap.transform.localScale);
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
        if (actualTrap != null && actualTrap.name == trapPrefab.name + "(Clone)")
        {
            Destroy(trap);
            actualTrap = null;
        }
        else
        {
            actualTrap = trapPrefab;
        }
    }

    private void CheckForBuilding()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, maxBuildingRange, BuildingLayers))
        {
            if (hit.transform != actualHit)
            {
                Destroy(trap);
                trap = actualTrap;
                Vector3 trapPosition;

                if (hit.transform.localScale.x < hit.transform.localScale.y)
                {
                    trapPosition = transform.position.x > hit.transform.position.x ? new Vector3(hit.transform.position.x + hit.transform.localScale.x, hit.transform.position.y, hit.transform.position.z) : new Vector3(hit.transform.position.x - hit.transform.localScale.x, hit.transform.position.y, hit.transform.position.z);
                }
                else
                {
                    trapPosition = transform.position.y > hit.transform.position.y ? new Vector3(hit.transform.position.x, hit.transform.position.y + hit.transform.localScale.y, hit.transform.position.z) : new Vector3(hit.transform.position.x, hit.transform.position.y - hit.transform.localScale.y, hit.transform.position.z);
                }

                trap = Instantiate(actualTrap, trapPosition, hit.transform.rotation);
                trap.transform.localScale = hit.transform.localScale;
                Color color = actualTrap.GetComponent<Renderer>().sharedMaterial.color;
                color.a = 0.1f;
                trap.GetComponent<Renderer>().sharedMaterial.color = color;
                actualHit = hit.transform;
                buildAllowed = true;
            }
        }
        else if (Vector3.Distance(actualTrap.transform.position, transform.position) > maxBuildingRange)
        {
            Destroy(trap);
            buildAllowed = false;
        }       

    }

    [PunRPC]
    private void RPCSetTrap(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject rpcTrap = Resources.Load(prefabName) as GameObject;
        rpcTrap = Instantiate(rpcTrap, position, rotation);
        rpcTrap.transform.localScale = scale;
        rpcTrap.GetComponent<Trap>().enabled = true;
    }
}
