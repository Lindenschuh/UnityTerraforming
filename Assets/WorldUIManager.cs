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
        player = GameObject.FindGameObjectWithTag("Player");
        camera = FindObjectOfType<vThirdPersonCamera>();
        pickupCanvas.SetActive(false);       
    }
	
	// Update is called once per frame
	void Update () {
        pickupCanvas.transform.rotation = camera.transform.rotation;
        if (pickupCanvas.active == true)
        {
           if (Input.GetKeyUp(KeyCode.E))
            {
                if(gameObject.GetComponent<ResourceObject>().inventoryIcon != null)
                {
                    player.GetComponent<ResourceControl>().PickupItem(gameObject);
                }
                ResourceObject resourceTaken = gameObject.GetComponent<ResourceObject>();
                player.GetComponent<ResourceControl>().AddResource(resourceTaken.resourceType, resourceTaken.resourceAmount);
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
