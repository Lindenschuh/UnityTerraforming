using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public float damagePerTick;
    public float tickTimer;
    public LayerMask enemies;
    public Vector3 direction;

    private float nextTickTime;
    private bool activated;
    private float timeLeft;

    private void Start()
    {
        activated = false;
        timeLeft = 0;
        if (direction.x > 0 && direction.y > 0 && direction.z < 0 && transform.parent.localRotation.y > 0)
            transform.localEulerAngles = new Vector3(0, 0, 90);
        else if (direction.x > 0 || (direction.x > 0 && direction.y > 0 && direction.z < 0 && transform.parent.localRotation.y == 0) || (direction.x < 0 && direction.y < 0 && direction.z > 0  && transform.parent.localRotation.y > 0))
            transform.localEulerAngles = new Vector3(0, 0, -90);
        else if (direction.x < 0)
            transform.localEulerAngles = new Vector3(0, 0, 90);
        else if (direction.y < 0)
            transform.localEulerAngles = new Vector3(0, 0, 180);
    }

    private void Update()
    {
        if (!activated)
        {
            Collider collider = transform.GetComponent<Collider>();
            RaycastHit[] BoxCastHit = Physics.BoxCastAll(transform.position, collider.bounds.size * 0.5f, direction, transform.localRotation, 4, enemies, QueryTriggerInteraction.Collide);
            if (BoxCastHit.Length > 0)
            {
                activated = true;
            }
        }

        if (activated)
        {
            timeLeft += Time.deltaTime;
            if (timeLeft >= nextTickTime)
            {
                DamageEnemies();
            }
        }    
    }

    private void DamageEnemies()
    {
        if (gameObject.GetComponentInChildren<ParticleSystem>())
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
        Collider collider = transform.GetComponent<Collider>();
        RaycastHit[] BoxCastHit = Physics.BoxCastAll(transform.position, collider.bounds.size * 0.5f, direction, transform.localRotation, 4, enemies, QueryTriggerInteraction.Collide);
        foreach (RaycastHit enemyHit in BoxCastHit)
        {
            enemyHit.transform.GetComponent<Health>().AddDamage((int)damagePerTick);
        }
        nextTickTime += tickTimer;
    }

}
