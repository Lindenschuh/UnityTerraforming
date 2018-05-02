using UnityEngine;

public class Flee : SteeringBehaviour
{
    private Transform target;
    private float maxAcceleration;

    public Flee(GameEntity character, Transform target, float maxAcceleration) : base(character)
    {
        this.target = target;
        this.maxAcceleration = maxAcceleration;
    }

    public override void CalculateForces()
    {
        direction = (Character.transform.position - target.position).normalized * maxAcceleration;
    }
}