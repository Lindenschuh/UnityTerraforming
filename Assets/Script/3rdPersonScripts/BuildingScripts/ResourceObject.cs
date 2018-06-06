using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour {
    public BuildResources resourceType;
    public int resourceAmount;
	// Use this for initialization
	void Start () {
        
    }
    private void Awake()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
