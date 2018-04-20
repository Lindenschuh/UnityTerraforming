using System;
using System.Collections;
using UnityEngine;

public class Enemy : GameEntity
{
    public override void CalculatePath()
    {
        _steeringBehaviour.Seek();
    }
}