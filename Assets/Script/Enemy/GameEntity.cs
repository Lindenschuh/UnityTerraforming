using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class GameEntity : MonoBehaviour
{
    public float Mass { get { return _rigidbody.mass; } set { _rigidbody.mass = value; } }
    public float MaxSpeed;
    public float MaxForce;
    public float SlowingRadius;
    public Transform Destination;

    protected Rigidbody _rigidbody;
    protected Vector3 _velocity;
    protected SteeringBehaviour _steeringBehaviour;

    public Vector3 Velocity { get { return _velocity; } private set { _velocity = value; } }

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _steeringBehaviour = new SteeringBehaviour(this);
    }

    private void FixedUpdate()
    {
        CalculatePath();
        _velocity += _steeringBehaviour.ApplySteering();
        _velocity = Vector3.ClampMagnitude(_velocity, MaxSpeed);
        _velocity = new Vector3(_velocity.x, 0, _velocity.z);
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
        if (_velocity != Vector3.zero)
        {
            _rigidbody.MoveRotation(Quaternion.LookRotation(_velocity));
        }
    }

    public abstract void CalculatePath();
}