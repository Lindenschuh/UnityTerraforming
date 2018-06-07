using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public float damagePerTick;
    public int ticks;
    public int duration;

    private float tickTimer;
    private float nextTickTime;
    private bool activated;
    private float timeLeft;

    private void Start()
    {
        activated = false;
        nextTickTime = duration / ticks;

        if (ticks == 1 && duration > 1)
            duration = 1;
    }

    private void Update()
    {
        if (activated && ticks > 0)
        {
            timeLeft -= Time.deltaTime;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activated)
        {
            activated = true;
        }
    }

}
