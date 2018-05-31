using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    private GameObject inventoryObject;
    private bool isInventoryActive;
	// Use this for initialization
	void Start () {
        inventoryObject = GameObject.Find("InventoryCanvas");
        isInventoryActive = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.I))
        {
            inventoryObject.SetActive(!isInventoryActive);
        }
	}
}
