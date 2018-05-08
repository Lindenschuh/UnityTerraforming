using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float MaxSpeed;
    public float MaxAccel;
    public float MaxRotation;
    public float MaxAngularAccel;
    public float Orientation;
    public float Rotation;
    public Vector3 Velocity;

    protected Steering steering;

    private void Start()
    {
        Velocity = Vector3.zero;
        steering = new Steering();
    }

    public void SetSteering(Steering steering, float weight)
    {
        this.steering.linear += (weight * steering.linear);
        this.steering.angualr += (weight * steering.angualr);
    }

    public virtual void Update()
    {
        Vector3 displacement = Velocity * Time.deltaTime;
        Orientation += Rotation * Time.deltaTime;

        if (Orientation < 0)
            Orientation += 360f;
        else if (Orientation > 360f)
            Orientation -= 360f;

        transform.Translate(displacement, Space.World);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, Orientation);
    }

    public virtual void LateUpdate()
    {
        Velocity += steering.linear * Time.deltaTime;
        Rotation += steering.angualr * Time.deltaTime;

        if (Velocity.magnitude > MaxSpeed)
        {
            Velocity.Normalize();
            Velocity = Velocity * MaxSpeed;
        }
        if (steering.angualr == 0)
        {
            Rotation = 0;
        }

        if (steering.linear.sqrMagnitude == 0)
        {
            Velocity = Vector3.zero;
        }

        steering = new Steering();
    }
}