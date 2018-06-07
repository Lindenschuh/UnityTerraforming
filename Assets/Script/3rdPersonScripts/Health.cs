using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int health;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {

            if (gameObject.layer == LayerMask.NameToLayer("BuildComponent"))
            {
                InteractionScript.Instance.CheckBuildings(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    
}
