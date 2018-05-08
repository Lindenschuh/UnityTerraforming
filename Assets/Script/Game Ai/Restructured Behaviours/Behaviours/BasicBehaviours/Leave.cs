using UnityEngine;

public class Leave : AgentBehaviour
{
    public float EscapeRadius;
    public float DangerRadius;
    public float TimeToTarget = 0.1f;

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        Vector3 direction = transform.position - Target.transform.position;
        float distance = direction.magnitude;

        if (distance > DangerRadius) return steering;

        float reduce;
        if (distance < EscapeRadius)
            reduce = 0;
        else
            reduce = distance / DangerRadius * Agent.MaxSpeed;

        float targetSpeed = Agent.MaxSpeed - reduce;

        Vector3 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= targetSpeed;

        steering.linear = desiredVelocity - Agent.Velocity;
        steering.linear /= TimeToTarget;

        steering.linear = Vector3.ClampMagnitude(steering.linear, Agent.MaxAccel);

        return steering;
    }
}