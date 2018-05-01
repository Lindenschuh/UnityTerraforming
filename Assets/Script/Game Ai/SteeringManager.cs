using UnityEngine;

public class SteeringManager
{
    private GameEntity _entity;
    private Vector3 _steering;

    private float _wanderAngle;

    public SteeringManager(GameEntity entity)
    {
        _entity = entity;
        _steering = new Vector3();
        _wanderAngle = 0;
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

    public void Wander(float wanderDistance, float wanderRadius, float angleChange, float weight = 1f)
    {
        _steering += DoWander(wanderDistance, wanderRadius, angleChange) * weight;
    }

    public void Evade(GameEntity target, float weight = 1f)
    {
        _steering += DoEvade(target) * weight - _entity.Velocity;
    }

    public void Persuit(GameEntity target, float weight = 1f)
    {
        _steering += DoPersuit(target) * weight - _entity.Velocity;
    }

    public void Avoid(Vector3 avoidPoint, float avoidanceForce, float weight = 1f)
    {
        _steering += avoidPoint.normalized * avoidanceForce;
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
        return -((targetPosition - _entity.transform.position).normalized * _entity.MaxVelocity);
    }

    private Vector3 DoWander(float wanderDistance, float wanderRadius, float angleChange)
    {
        var circleCenter = _entity.Velocity.normalized * wanderDistance;
        var displacement = new Vector3(Mathf.Cos(_wanderAngle) * wanderRadius, 0, Mathf.Sin(_wanderAngle) * wanderRadius);
        _wanderAngle = Random.value * angleChange - angleChange * .5f;

        circleCenter += displacement;
        return circleCenter;
    }

    private Vector3 DoEvade(GameEntity target)
    {
        var distance = target.transform.position - _entity.transform.position;
        var updatedNeeded = distance.magnitude / _entity.MaxVelocity;
        var futurePosition = target.transform.position + target.Velocity * updatedNeeded;
        return DoFlee(futurePosition);
    }

    private Vector3 DoPersuit(GameEntity target)
    {
        var distance = target.transform.position - _entity.transform.position;
        var updatesNeeded = distance.magnitude / _entity.MaxVelocity;
        var futurePosition = target.transform.position + target.Velocity * updatesNeeded;
        return DoSeek(futurePosition);
    }
}