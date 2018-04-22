using System;
using System.Collections;
using UnityEngine;

public class Enemy : GameEntity
{
    public override void CalculatePath()
    {
        _steeringBehaviour.Seek(Destination.transform.position, SlowingRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Destination")
        {
            Spawner.DestroyChild(this);
        }
    }
}