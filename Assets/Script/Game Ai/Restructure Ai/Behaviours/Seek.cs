using UnityEngine;

public class Seek : SteeringBehaviour
{
    private Transform target;
    private float maxAcceleration;

    public Seek(GameEntity character, Transform target, float maxAcceleration) : base(character)
    {
        this.target = target;
        this.maxAcceleration = maxAcceleration;
    }

    public override void CalculateForces()
    {
        direction = (target.position - Character.transform.position).normalized * maxAcceleration;
    }
}