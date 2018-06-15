using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int health;

    public void AddDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
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
        else
        {
            if (gameObject.tag == "Player")
            {
                GameObject.Find("HealthSlider").GetComponent<Slider>().value = health;
            }
        }
    }
}