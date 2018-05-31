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


    public void AddDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            BuildingDestroyer.Instance.CheckBuildings(gameObject);
        }
    }

    
}
