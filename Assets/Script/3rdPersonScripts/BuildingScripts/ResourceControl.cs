using Invector.vCamera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControl : MonoBehaviour {
    public int maxResourceAmount;
    public int resourceAmount;
    public LayerMask playerLayer;
    private LayerMask resLayer;
    private UIControl uiControl;
    public BuildResources[] resourceType;
    private GameObject head;
    protected Dictionary<BuildResources, int> resourceInfo;

    protected vThirdPersonCamera tpCamera;
    // Use this for initialization
    void Start () {
        tpCamera = FindObjectOfType<vThirdPersonCamera>();
        //resourceType = BuildResources.Wood;
        resourceInfo = new Dictionary<BuildResources, int>();
        resourceInfo.Add(BuildResources.Wood, 0);
        resourceInfo.Add(BuildResources.Trap, 0);
        //resourceInfo[resourceType] = 0;
        resLayer = LayerMask.NameToLayer("Resource");
        uiControl = GetComponent<UIControl>();
        head = GameObject.Find("Head");

        //Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetResourceInfo(BuildResources resType)
    {
        return resourceInfo[resType];
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(gameObject.layer) == "Player" && LayerMask.LayerToName(collision.gameObject.layer) == "Resource")
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
            TakeResource(collision);
        }

        
    }

    public void AddResource(BuildResources resourceType, int amount)
    {
        int oldAmount = resourceInfo[resourceType];
        resourceInfo[resourceType] += amount;
        uiControl.resetResourceCounter(resourceType, oldAmount, resourceInfo[resourceType]);
    }
    public void UseResource(BuildResources resourceType, int amount)
    {
        int oldAmount = resourceInfo[resourceType];
        resourceInfo[resourceType] -= amount;
        uiControl.resetResourceCounter(resourceType, oldAmount, resourceInfo[resourceType]);
    }
    private void TakeResource(Collision collision)
    {
        ResourceObject resourceTaken = collision.transform.gameObject.GetComponent<ResourceObject>();
        AddResource(resourceTaken.resourceType, resourceTaken.resourceAmount);
        collision.rigidbody.MovePosition(tpCamera.transform.position + tpCamera.transform.forward * 2);
        //collision.rigidbody.velocity = ((tpCamera.transform.position + tpCamera.transform.forward*2) - collision.transform.position) * 5;
        collision.rigidbody.MovePosition(head.transform.position);
        Destroy(collision.gameObject);
    }

   
}
