using Invector.vCamera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour {

    public GameObject pickupCanvas;
    private GameObject player;
    private vThirdPersonCamera camera;
    private bool takingResource;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = FindObjectOfType<vThirdPersonCamera>();
        pickupCanvas.SetActive(false);
        takingResource = false;
    }
	
	// Update is called once per frame
	void Update () {
        pickupCanvas.transform.rotation = camera.transform.rotation;
        if (pickupCanvas.GetActive())
        {
            if(Vector3.Distance(gameObject.transform.position, player.transform.position) > 20f)
            {
                pickupCanvas.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                ResourceObject resourceTaken = gameObject.GetComponent<ResourceObject>();
                player.GetComponent<ResourceControl>().AddResource(resourceTaken.resourceType, resourceTaken.resourceAmount);
                pickupCanvas.SetActive(false);
                takingResource = true;
            }
            
        }

        if (takingResource)
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) > 0.1f)
            {
                gameObject.GetComponent<Rigidbody>().MovePosition(player.transform.position);
            }
            else Destroy(gameObject);
        }
	}

    public void SetVisible(bool visible)
    {
        pickupCanvas.SetActive(visible);
    }
}
