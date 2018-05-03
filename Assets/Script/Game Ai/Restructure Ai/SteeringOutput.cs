using UnityEngine;

public struct SteeringOutput
{
    public Vector3 Linear;
    public float Angular;
    public float Weight;

    public SteeringOutput(Vector3 linear = new Vector3(), float angular = 0f, float weight = 1f)
    {
        Linear = linear;
        Angular = angular;
        Weight = weight;
    }
}