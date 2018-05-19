using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : MonoBehaviour {

    public int Health;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<vProjectileControl>() != null)
        {
            Health -= collision.gameObject.GetComponent<vProjectileControl>().damage.damageValue;
        }
        if(Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
