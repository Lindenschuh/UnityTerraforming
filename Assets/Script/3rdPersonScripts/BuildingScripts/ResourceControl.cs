using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControl : MonoBehaviour {
    public int maxResourceAmount;
    public int resourceAmount;
    public LayerMask playerLayer;
    private LayerMask resLayer;
    private UIControl uiControl;
    public BuildResources resourceType;
    private GameObject head;
    protected Dictionary<BuildResources, int> resourceInfo;
	// Use this for initialization
	void Start () {
        resourceType = BuildResources.Wood;
        resourceInfo = new Dictionary<BuildResources, int>();
        resourceInfo[resourceType] = 50;
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

            collision.rigidbody.velocity = (head.transform.position - collision.transform.position).normalized*5;
        Destroy(collision.gameObject);
    }

   
}
