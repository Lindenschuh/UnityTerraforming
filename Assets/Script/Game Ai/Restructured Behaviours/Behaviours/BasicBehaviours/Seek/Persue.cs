using UnityEngine;

public class Persue : Seek
{
    public float MaxPrediction;
    private GameObject targetAux;
    private Agent targetAgent;

    public override void Awake()
    {
        base.Awake();
        targetAgent = Target.GetComponent<Agent>();
        targetAux = Target;
        Target = new GameObject();
    }

    public override Steering GetSteering()
    {
        Vector3 direction = targetAux.transform.position - transform.position;
        float distance = direction.magnitude;
        float speed = Agent.Velocity.magnitude;

        float prediction;

        if (speed <= distance / MaxPrediction)
        {
            prediction = MaxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        Target.transform.position = targetAux.transform.position;
        Target.transform.position += targetAgent.Velocity * prediction;
        return base.GetSteering();
    }

    private void OnDestroy()
    {
        Destroy(targetAux);
    }
}