using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class NewGameEntity : MonoBehaviour
{
    public float MaxSpeed;
    public float RotationSpeed;
    public float Mass { get { return rb.mass; } set { rb.mass = value; } }

    protected List<SteeringBehaviour> behaviours;
    private Rigidbody rb;
    private Vector3 direction;
    private Vector3 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        behaviours.ForEach(be => be.CalculateForces());
        // apply Behaviours
        behaviours.ForEach(be => velocity += be.direction);

        // Trim Velocity to don't exceed the max speed
        velocity = (velocity.magnitude > MaxSpeed) ? velocity.normalized * MaxSpeed : velocity;
        rb.AddForce(velocity * Time.fixedTime);
        if (rb.velocity != Vector3.zero)
        {
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(rb.velocity.x, 0, rb.velocity.z)));
        }
    }

    public abstract void ApplyBehaviours();
}