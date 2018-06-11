using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour {
    public BuildResources resourceType;
    public int resourceAmount;
    public Sprite inventoryIcon;
    public GameObject objectPrefab;
	// Use this for initialization
	void Start () {
        
    }
    private void Awake()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"));
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Minimap"));
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("HeadTrack"));
    }
    // Update is called once per frame
    void Update () {
		
	}
}
