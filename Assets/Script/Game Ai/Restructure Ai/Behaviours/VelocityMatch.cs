using UnityEngine;

public class VelocityMatch : SteeringBehaviour
{
    private Vector3 targetVelocity;
    private float maxAcceleration;

    public VelocityMatch(GameEntity character, Vector3 targetVelocity, float maxAcceleration) : base(character)
    {
        this.targetVelocity = targetVelocity;
        this.maxAcceleration = maxAcceleration;
    }

    public override void CalculateForces()
    {
    }
}