using UnityEngine;

public static class Arrive
{
    private static float TIME_TO_TARGET = .1f;

    public static SteeringOutput CalculateForces(NewGameEntity character, Vector3 targetPosition)
    {
        var linear = character.transform.position - targetPosition;

        // Bin ich im target Radius

        // Bin ich im slow Radius

        return new SteeringOutput(linear: linear);
    }
}