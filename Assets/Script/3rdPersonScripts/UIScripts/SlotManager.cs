using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour {
    public GameObject objectInInventory;
    public int amount;
    public Vector3 slotPosition;
    public Transform parentObject;
    public BuildResources res;
	// Use this for initialization
	void Start () {
        
        amount = 0;
        slotPosition = gameObject.GetComponent<RectTransform>().position;
        parentObject = transform.parent;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset()
    {
        objectInInventory = null;
        amount = 0;
        GameObject.Find("UI").GetComponentInChildren<InventoryManager>(true).Reset();
    }
}
