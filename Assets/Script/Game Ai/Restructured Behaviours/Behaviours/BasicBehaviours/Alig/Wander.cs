using UnityEngine;

public class Wander : Face
{
    public float Offset;
    public float Radius;
    public float Rate;

    public override void Awake()
    {
        Target = new GameObject();
        Target.transform.position = transform.position;
        base.Awake();
    }

    public override Steering GetSteering()
    {
        Steering steering = new Steering();

        float wanderOrientation = Random.Range(-1f, 1f) * Rate;
        float targetOrientation = wanderOrientation + Agent.Orientation;

        Vector3 orientationVec = OriAsVector(Agent.Orientation);
        Vector3 targetPosition = (Offset * orientationVec) + transform.position;
        targetPosition = targetPosition + (OriAsVector(targetOrientation) * Radius);
        targetAux.transform.position = targetPosition;
        steering = base.GetSteering();
        steering.linear = targetAux.transform.position - transform.position;
        steering.linear.Normalize();
        steering.linear *= Agent.MaxAccel;
        return steering;
    }
}