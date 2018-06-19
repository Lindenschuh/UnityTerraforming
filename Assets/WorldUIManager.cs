using Invector.vCamera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour {

    public GameObject pickupCanvas;
    private GameObject player;
    private vThirdPersonCamera camera;
	// Use this for initialization
	void Start () {
        GameObject.FindGameObjectWithTag("Player");

        pickupCanvas.SetActive(false);       
    }
	
	// Update is called once per frame
	void Update () {
        pickupCanvas.transform.rotation = Camera.main.transform.rotation;
        if (pickupCanvas.active == true)
        {
           if (Input.GetKeyUp(KeyCode.E))
            {
                if(gameObject.GetComponent<ResourceObject>().inventoryIcon != null)
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<ResourceControl>().PickupItem(gameObject);
                }
                ResourceObject resourceTaken = gameObject.GetComponent<ResourceObject>();
                GameObject.FindGameObjectWithTag("Player").GetComponent<ResourceControl>().AddResource(resourceTaken.resourceType, resourceTaken.resourceAmount);
                pickupCanvas.SetActive(false);
                Destroy(gameObject);
            }           
        }      
	}

    public void SetVisible(bool visible)
    {
        pickupCanvas.SetActive(visible);
    }
}
