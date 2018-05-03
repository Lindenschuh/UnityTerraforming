using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class NewGameEntity : MonoBehaviour
{
    public float MaxSpeed;
    public float MaxAcceleration;
    public float RotationSpeed;
    public float MaxRotation;
    public float SlowRadius;
    public float TargetRadius;

    [HideInInspector]
    public float Mass { get { return rb.mass; } set { rb.mass = value; } }

    protected List<SteeringOutput> outputs;
    private Rigidbody rb;
    private Vector3 velocity;
    private float rotation;

    private Vector3 Destination;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        outputs = new List<SteeringOutput>();
        velocity = new Vector3();
        rotation = 0f;
    }

    private void FixedUpdate()
    {
        ApplyBehaviours();

        outputs.ForEach(o => velocity += o.Linear * o.Weight);

        // Trim Velocity to don't exceed the max speed
        velocity = (velocity.magnitude > MaxSpeed) ? velocity.normalized * MaxSpeed : velocity;
        rotation = (rotation > MaxRotation) ? MaxRotation : rotation;
        rb.AddForce(velocity * Time.fixedTime);
        if (rb.velocity != Vector3.zero)
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(rb.velocity.x, 0, rb.velocity.z)));
    }

    public abstract void ApplyBehaviours();
}