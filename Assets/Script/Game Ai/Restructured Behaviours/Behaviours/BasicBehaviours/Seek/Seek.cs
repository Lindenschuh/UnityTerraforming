using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Seek : AgentBehaviour
{
    public override Steering GetSteering()
    {
        Steering steering = new Steering
        {
            linear = Target.transform.position - transform.position
        };

        steering.linear.Normalize();
        steering.linear = steering.linear * Agent.MaxAccel;
        return steering;
    }
}