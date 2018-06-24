using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public float damagePerTick;
    public float tickTimer;
    public LayerMask enemies;

    private float nextTickTime;
    private bool activated;
    private float timeLeft;

    private void Start()
    {
        activated = false;
        timeLeft = 0;
    }

    private void Update()
    {
        if (!activated)
        {
            RaycastHit[] BoxCastHit = Physics.BoxCastAll(transform.position, transform.localScale, transform.up, transform.localRotation, 4, enemies, QueryTriggerInteraction.Collide);
            if (BoxCastHit.Length > 0)
                activated = true;
        }

        if (activated)
        {
            timeLeft += Time.deltaTime;
            if (timeLeft >= tickTimer)
            {
                DamageEnemies();
            }
        }    
    }

    private void DamageEnemies()
    {
        RaycastHit[] BoxCastHit = Physics.BoxCastAll(transform.position, transform.localScale, transform.up, transform.localRotation, 4, enemies, QueryTriggerInteraction.Collide);
        foreach (RaycastHit enemyHit in BoxCastHit)
        {
            enemyHit.transform.GetComponent<Health>().AddDamage((int)damagePerTick);
        }
        tickTimer += nextTickTime;
    }

}
