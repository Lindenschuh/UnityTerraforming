using UnityEngine;

public class Arrive : AgentBehaviour
{
    public float TargetRadius;
    public float SlowRadius;
    public float TimeToTarget = 0.1f;

    public override Steering GetSteering()
    {
        //if (Target == null)
        //    Target = Agent.GetAriveDestination();

        Steering steering = new Steering();
        Vector3 direction = Target.transform.position - transform.position;
        float distance = direction.magnitude;
        float targetSpeed;
        if (distance < TargetRadius) return steering;
        if (distance > SlowRadius)
            targetSpeed = Agent.MaxSpeed;
        else
            targetSpeed = Agent.MaxSpeed * distance / SlowRadius;

        Vector3 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= targetSpeed;

        steering.linear = desiredVelocity - Agent.Velocity;
        steering.linear /= TimeToTarget;

        steering.linear = Vector3.ClampMagnitude(steering.linear, Agent.MaxAccel);

        return steering;
    }
}