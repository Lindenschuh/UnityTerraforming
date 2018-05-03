using UnityEngine;

public static class Seek
{
    public static SteeringOutput CalculateForces(NewGameEntity character, Vector3 target)
    {
        var linear = (character.transform.position - target).normalized * character.MaxAcceleration;

        return new SteeringOutput(linear: linear);
    }
}