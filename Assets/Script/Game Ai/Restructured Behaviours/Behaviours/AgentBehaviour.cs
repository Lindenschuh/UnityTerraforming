using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    public GameObject Target;
    public Agent Agent;
    public float Weight = 1f;

    public virtual void Awake()
    {
        Agent = gameObject.GetComponent<Agent>();
    }

    public virtual void Update()
    {
        Agent.SetSteering(GetSteering(), Weight);
    }

    public virtual Steering GetSteering()
    {
        return new Steering();
    }

    public float MapToRange(float rotation)
    {
        rotation %= 360f;

        if (Mathf.Abs(rotation) > 180f)
        {
            if (rotation < 0)
                rotation += 360f;
            else
                rotation -= 360f;
        }
        return rotation;
    }

    public Vector3 OriAsVector(float orientation)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1f;
        return vector.normalized;
    }
}