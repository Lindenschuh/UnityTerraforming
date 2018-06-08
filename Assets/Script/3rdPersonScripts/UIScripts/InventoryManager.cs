using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public List<GameObject> slotList;
	// Use this for initialization
	void Start () {
        slotList = new List<GameObject>();
        slotList.Add(GameObject.Find("InventorySlot"));
        for(int i = 1; i <9; i++)
        {
            slotList.Add(GameObject.Find("InventorySlot (" + i + ")"));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
