using System;
using System.Collections;
using UnityEngine;

public class Enemy : GameEntity
{
    [HideInInspector]
    public GameEntity Destination;

    [Range(0, MAX_RANGE)]
    public float SeekingWeight;

    private void Awake()
    {
        Destination = GameManager.instance.MainDestination;
    }

    public override void CalculatePath()
    {
        _steeringBehaviour.Seek(Destination.transform.position, SlowingRadius, SeekingWeight);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Destination")
        {
            Spawner.DestroyChild(this);
        }
    }
}