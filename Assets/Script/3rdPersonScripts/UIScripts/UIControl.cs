using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    public GameObject wallIcon;
    public GameObject groundIcon;
    public GameObject rampIcon;

    public GameObject selectedUIElement;

    private GameObject uiCanvas;
    private BuildMode buildScript;
    private GameObject woodCount;
    public Dictionary<BuildResources, int> resourceInfo;
    private ResourceControl resControl;
    private bool isCounting;
    private GameObject inventoryObject;
    private vShooterMeleeInput inputScript;
    // Use this for initialization
    void Start () {
        buildScript = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildMode>();
        resControl = GetComponent<ResourceControl>();
        resourceInfo = new Dictionary<BuildResources, int>();
        resourceInfo.Add(BuildResources.Wood, 0);
        woodCount = GameObject.Find("WoodCount");
        isCounting = false;
        inventoryObject = GameObject.Find("InventoryPanel");

        inventoryObject.SetActive(false);

        inputScript = GetComponent<vShooterMeleeInput>();
        uiCanvas = GameObject.Find("UICanvas");
    }
	
	// Update is called once per frame
	void Update () {

        HandleUIInteraction();
        HandleInventory();
        HandleBuildKeys();
    }

    public void resetResourceCounter(BuildResources resourceType,int oldAmount, int newAmount)
    {
        switch (resourceType)
        {
            case BuildResources.Wood:
                if (!isCounting)
                {
                    StartCoroutine(Wait(oldAmount));
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
            inventoryObject.SetActive(!inventoryObject.GetActive());
            inventoryObject.GetComponentInChildren<UIComponentControl>().enabled = inventoryObject.GetActive();
            uiCanvas.SetActive(!inventoryObject.GetActive());
            inputScript.lockInput = inventoryObject.GetActive();
            inputScript.LockCursor(inventoryObject.GetActive());
            inputScript.ShowCursor(inventoryObject.GetActive());
            inputScript.SetLockCameraInput(inventoryObject.GetActive());
        }
        if (inventoryObject.GetActive())
        {
           
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
    IEnumerator Wait(int oldAmount)
    {
        isCounting = true;
            while (resControl.GetResourceInfo(BuildResources.Wood) != oldAmount )
            {
            if (resControl.GetResourceInfo(BuildResources.Wood) < oldAmount) oldAmount--;
            else oldAmount++;
            yield return new WaitForSeconds(0.05f);
                woodCount.GetComponent<Text>().text = oldAmount.ToString();

            }
        
        isCounting = false;
        }
    
}
