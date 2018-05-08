using System;
using System.Collections.Generic;
using UnityEngine;

public class AvoidWall : Seek
{
    public float AvoidDistance;
    public float LookAhead;

    public float FeelerAngle;
    public float FeelerScale = 2;

    public override void Awake()
    {
        base.Awake();
        Target = new GameObject();
    }

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        Vector3 position = transform.position;
        Vector3 rayVector = Agent.Velocity.normalized * LookAhead;
        Vector3 direction = rayVector;
        Vector3 directionRight = Quaternion.AngleAxis(FeelerAngle, Vector3.up) * rayVector;
        Vector3 directionLeft = Quaternion.AngleAxis(-FeelerAngle, Vector3.up) * rayVector;
        RaycastHit hit;

        Debug.DrawRay(position, direction, Color.green);
        Debug.DrawRay(position, directionLeft / FeelerScale, Color.red);
        Debug.DrawRay(position, directionRight / FeelerScale, Color.blue);

        if (Physics.Raycast(position, direction, out hit, LookAhead))
        {
            Target.transform.position = hit.point + hit.normal * AvoidDistance; ;
            Debug.DrawRay(hit.point, hit.normal * AvoidDistance, Color.yellow);
            return base.GetSteering();
        }
        if (Physics.Raycast(position, directionLeft, out hit, LookAhead / FeelerScale))
        {
            Target.transform.position = hit.point + hit.normal * AvoidDistance; ;
            Debug.DrawRay(hit.point, hit.normal * AvoidDistance, Color.yellow);
            return base.GetSteering();
        }

        if (Physics.Raycast(position, directionRight, out hit, LookAhead / FeelerScale))
        {
            Target.transform.position = hit.point - hit.normal * AvoidDistance; ;
            Debug.DrawRay(hit.point, hit.normal * AvoidDistance, Color.yellow);
            return base.GetSteering();
        }

        return steering;
    }
}