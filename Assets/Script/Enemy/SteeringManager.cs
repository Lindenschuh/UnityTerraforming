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

    public Vector3 ApplySteering()
    {
        _steering = Vector3.ClampMagnitude(_steering, _entity.MaxForce);
        _steering /= _entity.Mass;

        var force = _steering;
        _steering = new Vector3();
        return force;
    }

    public void Seek()
    {
        var desiredForce = _entity.Destination.position - _entity.transform.position;

        var distance = desiredForce.magnitude;

        desiredForce.Normalize();

        if (distance <= _entity.SlowingRadius)
        {
            desiredForce *= _entity.MaxSpeed * (distance / _entity.SlowingRadius);
        }
        else
        {
            desiredForce *= _entity.MaxSpeed;
        }

        _steering += desiredForce - _entity.Velocity;
    }
}