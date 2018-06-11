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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetResourceInfo(BuildResources resType)
    {
        return resourceInfo[resType];
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

    private void PickItem(GameObject item)
    {
        item.GetComponent<Rigidbody>().MovePosition(head.transform.position);



        Destroy(item);
    }
}
