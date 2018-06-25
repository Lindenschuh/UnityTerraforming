using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
    public List<GameObject> slotList;

    private GameObject selectedTrapSlot;
    private int selectedCount;
	// Use this for initialization
	void Start () {
        selectedCount = 0;
        //slotList = new List<GameObject>();
        //slotList.Add(GameObject.Find("InventorySlot"));
        //for(int i = 1; i <9; i++)
        //{
        //  slotList.Add(GameObject.Find("InventorySlot (" + i + ")"));
        //}
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void Reset()
    {
            for (int i = 0; i < slotList.Count; i++)
            {
                if (slotList[i].GetComponent<SlotManager>().objectInInventory != null)
                {
                    selectedTrapSlot = slotList[i];
                    selectedCount = i;
                GameObject.Find("TrapPanel").GetComponent<Image>().sprite = selectedTrapSlot.GetComponent<Image>().sprite;
                return;
                }
            }
            selectedTrapSlot = null;
        GameObject.Find("TrapPanel").GetComponent<Image>().sprite = null;
    }
    public void PickupItem(GameObject item)
    {

        for (int i = 0; i < slotList.Count; i++)
        {
            
            if (slotList[i].GetComponent<SlotManager>().objectInInventory == null)
            {

                slotList[i].GetComponent<Image>().sprite = item.GetComponent<ResourceObject>().inventoryIcon;
                slotList[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
                slotList[i].GetComponent<SlotManager>().objectInInventory = item.GetComponent<ResourceObject>().objectPrefab;
                slotList[i].GetComponent<SlotManager>().res = item.GetComponent<ResourceObject>().resourceType;
                if (selectedTrapSlot == null)
                {
                    selectedTrapSlot = slotList[i];
                    selectedCount = i;
                    GameObject.Find("TrapPanel").GetComponent<Image>().sprite = selectedTrapSlot.GetComponent<Image>().sprite;
                }
                return;
            }
        }
    }

    public GameObject GetSelectedTrap()
    {
        if (selectedTrapSlot != null)
            return selectedTrapSlot.GetComponent<SlotManager>().objectInInventory;
        else
            return null;
    }

    public void RemoveTrap()
    {
        selectedTrapSlot.GetComponent<Image>().sprite = null;
        selectedTrapSlot.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0);
        selectedTrapSlot.GetComponent<SlotManager>().Reset();
    }

    public void SelectNextTrap()
    {
        for(int i = selectedCount; i < slotList.Count; i++)
        {
            if(slotList[i].GetComponent<Image>().sprite != null)
            {
                selectedTrapSlot = slotList[i];
                return;
            }
        }

        for(int i = 0; i < selectedCount; i++)
        {
            if (slotList[i].GetComponent<Image>().sprite != null)
            {
                selectedTrapSlot = slotList[i];
                return;
            }
        }
    }
}
