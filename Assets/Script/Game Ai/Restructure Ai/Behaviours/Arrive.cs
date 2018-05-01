using UnityEngine;

public class Arrive : SteeringManager
{
    private Transform target;
    private float maxAcceleration;
    private float targetRadius;
    private float slowRadius;

    public Arrive(GameEntity character, Transform target, float maxAcceleration, float targetRadius, float slowRadius) : base(character)
    {
        this.target = target;
        this.maxAcceleration = maxAcceleration;
        this.targetRadius = targetRadius;
        this.slowRadius = slowRadius;
    }

    public override void CalculateForces()
    {
        var dir = target.position - Character.transform.position;
        var distance = dir.magnitude;

        if (distance < targetRadius) return;

        direction = dir.normalized * maxAcceleration;
        if (distance <= slowRadius) direction *= (distance / slowRadius);
    }
}