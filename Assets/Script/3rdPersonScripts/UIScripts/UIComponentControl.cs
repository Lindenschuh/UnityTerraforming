using Invector.vCamera;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIComponentControl : Photon.PunBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject woodRes;
    public GameObject instantTrap;
    public GameObject tickTrap;
    private bool isEntered;
    private GameObject movableUIComp;
    private GameObject dropAmountInput;
    private GameObject player;
    private GameObject objectToDrop;
    private UIControl uiControl;
    private Vector3 position;
    private vThirdPersonCamera tpCamera;

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        Debug.Log("Name: " + eventData.pointerCurrentRaycast.gameObject.name);
        Debug.Log("Tag: " + eventData.pointerCurrentRaycast.gameObject.tag);
        Debug.Log("GameObject: " + eventData.pointerCurrentRaycast.gameObject);
        isEntered = true;
        if (eventData.pointerCurrentRaycast.gameObject.tag == "MovableUI"){
            position = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotManager>().slotPosition;
            movableUIComp = eventData.pointerCurrentRaycast.gameObject;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            isEntered = false;
            movableUIComp = null;
        }
    }

    public void DropAmountChanged(Text input)
    {
        int realAmount = 0;
        string text = input.GetComponent<Text>().text ;
        if (int.TryParse(text, out realAmount))
        {
            if (realAmount > uiControl.resourceInfo[BuildResources.Wood])
            {
                realAmount = uiControl.resourceInfo[BuildResources.Wood];
                dropAmountInput.GetComponent<InputField>().text = realAmount.ToString();
            }
        }

        if(realAmount > 0)
        {
            
            Vector3 dropPosition = tpCamera.transform.position + tpCamera.transform.forward * 5;
            photonView.RPC("RPCDropResources", PhotonTargets.All,BuildResources.Wood , realAmount, dropPosition);

            player.GetComponent<ResourceControl>().UseResource(BuildResources.Wood, realAmount);
        }
        dropAmountInput.GetComponent<InputField>().text = "";
        dropAmountInput.SetActive(false);
    }

    private void DropItem(GameObject item, int amount)
    {
        Vector3 dropPosition = tpCamera.transform.position + tpCamera.transform.forward * 5;
        GameObject instItem = Instantiate(item);
        instItem.transform.position = dropPosition;
        movableUIComp.GetComponent<Image>().sprite = null;
        movableUIComp.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0);
        movableUIComp.GetComponent<SlotManager>().Reset();
    }
    // Use this for initialization
    void Start () {
        tpCamera = FindObjectOfType<vThirdPersonCamera>();
        player = GameObject.FindGameObjectWithTag("Player");
        dropAmountInput = GameObject.Find("DropAmount");
        dropAmountInput.GetComponent<InputField>().onEndEdit.AddListener(delegate { DropAmountChanged(dropAmountInput.GetComponent<InputField>().textComponent); });
        dropAmountInput.SetActive(false);
        uiControl = player.GetComponent<UIControl>();
        isEntered = false;
        
	}
	
	// Update is called once per frame
	void Update () {

        if(movableUIComp != null)
        {
            DragActions();
        }
        if (dropAmountInput.GetActive())
        {
            

        }
    }

    private void DragActions()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            movableUIComp.transform.position = Input.mousePosition;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(Vector3.Distance(movableUIComp.GetComponent<RectTransform>().position, position) > 50f)
            {
                if (movableUIComp.GetComponent<SlotManager>().objectInInventory != null)
                {
                    if (movableUIComp.GetComponent<SlotManager>().objectInInventory.tag == "BuildResource")
                    {
                        objectToDrop = movableUIComp.GetComponent<SlotManager>().objectInInventory;
                        dropAmountInput.SetActive(true);

                    }else
                    {
                        switch (movableUIComp.GetComponent<SlotManager>().res)
                        {
                            case BuildResources.TrapInstant:
                                DropItem(instantTrap, 1);
                                
                                break;
                            case BuildResources.TrapTick:
                                DropItem(tickTrap, 1);
                                break;
                        }
                    }
                }
            }
            movableUIComp.transform.SetParent(movableUIComp.GetComponent<SlotManager>().parentObject,false);
            movableUIComp.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

        }
    }

    #region PunRPC

    [PunRPC]
    private void RPCDropResources(BuildResources resourceType, int amount, Vector3 position)
    {
        GameObject instRes = null;
        switch (resourceType)
        {
            case BuildResources.Wood:
               instRes = Instantiate(woodRes);

                break;
            default:
                return;
        }
        instRes.transform.position = tpCamera.transform.position + tpCamera.transform.forward * 5;
        instRes.GetComponent<ResourceObject>().resourceAmount = amount;
    }

    #endregion
}
