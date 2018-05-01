using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : GameEntity
{
    public float WanderRadius;
    public float WanderDistance;
    public float AngleChange;

    public override void CalculatePath()
    {
        _steeringBehaviour.Wander(WanderDistance, WanderRadius, AngleChange, .5f);
    }
}