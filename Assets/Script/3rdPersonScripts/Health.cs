using Invector;
using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public int health;


	// Use this for initialization
	void Start () {
        gameObject.GetComponent<vHealthController>().onDead.AddListener(OnDeath);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnDeath(GameObject gameobjct)
    {
        if (gameObject.layer == LayerMask.NameToLayer("BuildComponent"))
        {
            InteractionScript.Instance.CheckBuildings(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (gameObject.tag == "Player")
        {
            GameObject.Find("HealthSlider").GetComponent<Slider>().value = health;
        }
    }
   
    public void AddDamage(int damage)
    {
        
        health -= damage;

        if(health <= 0)
        {
            Destroy(gameObject);

        }else
        {

        }
    }


}
