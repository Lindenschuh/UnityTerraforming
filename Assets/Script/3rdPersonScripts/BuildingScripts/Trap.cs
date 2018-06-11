using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public float damagePerTick;
    public int ticks;
    public int duration;
    public LayerMask enemies;

    private float tickTimer;
    private float nextTickTime;
    private bool activated;
    private float timeLeft;

    private void Start()
    {
        activated = false;
        nextTickTime = duration / ticks;
        tickTimer = nextTickTime;
        timeLeft = 0;

        if (ticks == 1 && duration > 1)
            duration = 1;
    }

    private void Update()
    {
        if (!activated)
        {
            RaycastHit[] BoxCastHit = Physics.BoxCastAll(transform.position, transform.localScale, transform.up, transform.localRotation, 4, enemies, QueryTriggerInteraction.Collide);
            if (BoxCastHit.Length > 0)
                activated = true;
        }

        if (activated && ticks > 0)
        {
            timeLeft += Time.deltaTime;
            if (timeLeft >= tickTimer)
            {
                DamageEnemies();
            }

        }

        if (ticks <= 0)
            Destroy(gameObject);
        
    }

    private void DamageEnemies()
    {
        RaycastHit[] BoxCastHit = Physics.BoxCastAll(transform.position, transform.localScale, transform.up, transform.localRotation, 4, enemies, QueryTriggerInteraction.Collide);
        foreach (RaycastHit enemyHit in BoxCastHit)
        {
            enemyHit.transform.GetComponent<Health>().AddDamage((int)damagePerTick);
        }
        ticks--;
        tickTimer += nextTickTime;
    }

}
