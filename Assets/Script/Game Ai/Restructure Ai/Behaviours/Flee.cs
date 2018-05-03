using UnityEngine;

public static class Flee
{
    public static SteeringOutput CalculateForces(NewGameEntity character, Vector3 targetPosition)
    {
        var linear = (targetPosition - character.transform.position).normalized * character.MaxAcceleration;
        return new SteeringOutput(linear: linear);
    }
}