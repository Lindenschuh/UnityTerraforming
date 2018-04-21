using System;
using UnityEngine;

public class SteeringBehaviour
{
    private GameEntity _entity;
    private Vector3 _steering;

    public SteeringBehaviour(GameEntity entity)
    {
        _entity = entity;
        _steering = new Vector3();
    }

    public Vector3 UpdateSteering()
    {
        _steering = Vector3.ClampMagnitude(_steering, _entity.MaxForce);
        _steering /= _entity.Mass;

        return _steering;
    }

    public void ResetSteering()
    {
        _steering = new Vector3();
    }

    public void Seek(Vector3 targetPosition, float slowingRadius, float weight = 1f)
    {
        _steering += DoSeek(targetPosition, slowingRadius) * weight - _entity.Velocity;
    }

    public void Flee(Vector3 targetPosition, float weight = 1f)
    {
        _steering += DoFlee(targetPosition) * weight - _entity.Velocity;
    }

    public void Wander(float wanderRadius, float weight = 1f)
    {
        _steering += DoWander(wanderRadius) * weight - _entity.Velocity;
    }

    public void Evade(GameEntity target, float weight = 1f)
    {
        _steering += DoEvade(target) * weight - _entity.Velocity;
    }

    public void Persuit(GameEntity target, float weight = 1f)
    {
        _steering += DoPersuit(target) * weight - _entity.Velocity;
    }

    private Vector3 DoSeek(Vector3 targetPosition, float slowingRadius = 0f)
    {
        var desiredForce = targetPosition - _entity.transform.position;

        var distance = desiredForce.magnitude;

        desiredForce.Normalize();

        if (distance <= slowingRadius)
        {
            desiredForce *= _entity.MaxVelocity * (distance / slowingRadius);
        }
        else
        {
            desiredForce *= _entity.MaxVelocity;
        }

        return desiredForce;
    }

    private Vector3 DoFlee(Vector3 targetPosition)
    {
        return (targetPosition - _entity.transform.position).normalized * _entity.MaxVelocity;
    }

    private Vector3 DoWander(float wanderRadius)
    {
        return new Vector3();
    }

    public Vector3 DoEvade(GameEntity target)
    {
        return DoFlee(target.transform.position);
    }

    private Vector3 DoPersuit(GameEntity target)
    {
        return DoSeek(target.transform.position);
    }
}