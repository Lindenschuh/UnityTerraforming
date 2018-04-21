using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : GameEntity
{
    public float WanderRadius;

    [Range(1f, 10f)]
    public float WanderPointRate;

    private float nextWanderTime = 0;
    private Vector3 cuurentTarget;

    public override void CalculatePath()
    {
        if (Time.time > nextWanderTime)
        {
            nextWanderTime = Time.time + WanderPointRate;
            cuurentTarget = _steeringBehaviour.NextWanderTarget(WanderRadius);
        }

        _steeringBehaviour.Seek(cuurentTarget, SlowingRadius);
    }
}