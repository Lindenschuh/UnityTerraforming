using UnityEngine;

public abstract class SteeringBehaviour
{
    // Need to be public so they can be accesed by the Ai Script
    public float Weight;

    public Vector3 direction;

    public GameEntity Character;

    public SteeringBehaviour(GameEntity character, float weight = 1f)
    {
        direction = new Vector3();
        Weight = weight;
        Character = character;
    }

    public abstract void CalculateForces();
}