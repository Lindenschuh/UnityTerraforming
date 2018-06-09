using Invector.vCamera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour {

    private GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {


        gameObject.transform.rotation = GameObject.Find("vThirdPersonCamera").transform.rotation;
	}
}
