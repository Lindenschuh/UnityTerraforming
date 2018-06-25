using Invector.vCamera;
using Invector.vShooter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    private GameObject wallIcon;
    private GameObject groundIcon;
    private GameObject rampIcon;

    public GameObject selectedUIElement;

    private GameObject uiCanvas;
    private BuildMode buildScript;
    private GameObject woodCountUI;
    private GameObject woodCountInventory;
    private GameObject trapCountUI;
    private GameObject trapCountInventory;
    public Dictionary<BuildResources, int> resourceInfo;
    private ResourceControl resControl;
    private bool isCounting;
    private GameObject inventoryObject;
    private vShooterMeleeInput inputScript;
    private vThirdPersonCamera thirdPCamera;
    private GameObject visiblePickupItem;
    
    // Use this for initialization
    void OnEnable () {
        buildScript = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildMode>();
        resControl = GetComponent<ResourceControl>();
        resourceInfo = new Dictionary<BuildResources, int>();
        resourceInfo.Add(BuildResources.Wood, 0);
        resourceInfo.Add(BuildResources.TrapFire, 0);
        resourceInfo.Add(BuildResources.TrapIce, 0);
        woodCountUI = GameObject.Find("WoodCount");
        woodCountInventory = GameObject.Find("WoodCountInventory");
        trapCountUI = GameObject.Find("TrapCount");
        trapCountInventory = GameObject.Find("TrapCountInventory");
        isCounting = false;
        inventoryObject = GameObject.Find("InventoryPanel");

        inventoryObject.SetActive(false);

        inputScript = GetComponent<vShooterMeleeInput>();
        uiCanvas = GameObject.Find("UICanvas");
        wallIcon = GameObject.Find("WallPanel");
        groundIcon = GameObject.Find("GroundPanel");
        rampIcon = GameObject.Find("RampPanel");
        thirdPCamera = FindObjectOfType<vThirdPersonCamera>();
        visiblePickupItem = null;
    }
	
	// Update is called once per frame
	void Update () {

        HandleUIInteraction();
        HandleInventory();
        HandleBuildKeys();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (visiblePickupItem != null && hit.collider.gameObject != visiblePickupItem)
            {
                visiblePickupItem.GetComponentInChildren<WorldUIManager>().SetVisible(false);
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Resource"))
            {
                visiblePickupItem = hit.collider.gameObject;
                visiblePickupItem.GetComponentInChildren<WorldUIManager>().SetVisible(true);
            }
            else visiblePickupItem = null;
        }
    }

    public void resetResourceCounter(BuildResources resourceType,int oldAmount, int newAmount)
    {
        switch (resourceType)
        {
            case BuildResources.Wood:
                resourceInfo[resourceType] = newAmount;
                if (!isCounting)
                {
                    StartCoroutine(Wait(oldAmount, resourceType, woodCountUI));
                }             
                break;
            case BuildResources.TrapFire:
                resourceInfo[resourceType] = newAmount;
                if (!isCounting)
                {
                    //StartCoroutine(Wait(oldAmount, resourceType, trapCountUI));
                }
                break;
            case BuildResources.TrapIce:
                resourceInfo[resourceType] = newAmount;
                if (!isCounting)
                {
                    //StartCoroutine(Wait(oldAmount, resourceType, trapCountUI));
                }
                break;
        }
    }

    private void HandleBuildKeys()
    {
        if (Input.GetKeyUp(buildScript.groundKey))
        {
            if (groundIcon.GetComponent<Image>().color == Color.green) groundIcon.GetComponent<Image>().color = Color.white;
            else groundIcon.GetComponent<Image>().color = Color.green;
            wallIcon.GetComponent<Image>().color = Color.white;
            rampIcon.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyUp(buildScript.wallKey))
        {
            if (wallIcon.GetComponent<Image>().color == Color.green) wallIcon.GetComponent<Image>().color = Color.white;
            else wallIcon.GetComponent<Image>().color = Color.green;
            groundIcon.GetComponent<Image>().color = Color.white;
            rampIcon.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyUp(buildScript.rampKey))
        {
            if (rampIcon.GetComponent<Image>().color == Color.green) rampIcon.GetComponent<Image>().color = Color.white;
            else rampIcon.GetComponent<Image>().color = Color.green;

            groundIcon.GetComponent<Image>().color = Color.white;
            wallIcon.GetComponent<Image>().color = Color.white;
        }
    }

    private void HandleInventory()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            inventoryObject.SetActive(!inventoryObject.active);
            //inventoryObject.GetComponentInChildren<UIComponentControl>().enabled = inventoryObject.GetActive();
            //uiCanvas.SetActive(!inventoryObject.GetActive());
            inputScript.lockInput = inventoryObject.active;
            inputScript.LockCursor(inventoryObject.active);
            inputScript.ShowCursor(inventoryObject.active);
            inputScript.SetLockCameraInput(inventoryObject.active);
        }
        if (inventoryObject.GetActive())
        {
            //woodCountInventory.GetComponent<Text>().text = resourceInfo[BuildResources.Wood].ToString();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    bool isOver = true;
                }
                
            }
        }
    }

    private void HandleUIInteraction()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            inputScript.lockInput = true;
            inputScript.LockCursor(true);
            inputScript.ShowCursor(true);
            inputScript.SetLockCameraInput(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            inputScript.lockInput = false;
            inputScript.LockCursor(false);
            inputScript.ShowCursor(false);
            inputScript.SetLockCameraInput(false);
        }
    }


    IEnumerator Wait(int oldAmount, BuildResources resource, GameObject uiControll)
    {
        isCounting = true;
            while (resControl.GetResourceInfo(resource) != oldAmount )
            {
            if (resControl.GetResourceInfo(resource) < oldAmount) oldAmount--;
            else oldAmount++;
            yield return new WaitForSeconds(0.05f);
                uiControll.GetComponent<Text>().text = oldAmount.ToString();

            }
        
        isCounting = false;
        }
    
}
