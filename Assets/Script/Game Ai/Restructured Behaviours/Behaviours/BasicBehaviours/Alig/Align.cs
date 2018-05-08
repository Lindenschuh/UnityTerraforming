using UnityEngine;

public class Align : AgentBehaviour
{
    public float TargetRadius;
    public float SlowRadius;
    public float TimeToTarget = 0.1f;

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        float targetOrientation = Target.GetComponent<Agent>().Orientation;
        float rotation = targetOrientation - Agent.Orientation;

        rotation = MapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);
        if (rotationSize < TargetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > SlowRadius)
            targetRotation = Agent.MaxRotation;
        else
            targetRotation = Agent.MaxRotation * rotationSize / SlowRadius;

        targetRotation *= rotation / rotationSize;
        steering.angualr = targetRotation - Agent.Rotation;
        steering.angualr /= TimeToTarget;
        float angularAccel = Mathf.Abs(steering.angualr);
        if (angularAccel > Agent.MaxAngularAccel)
        {
            steering.angualr /= angularAccel;
            steering.angualr *= Agent.MaxAngularAccel;
        }
        return steering;
    }
}