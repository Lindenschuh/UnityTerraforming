using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityTerraforming.GameAi
{
    public class Flee : AgentBehaviour
    {
        public override Steering GetSteering()
        {
            Steering steering = new Steering
            {
                linear = transform.position - Target.transform.position
            };
            steering.linear.Normalize();
            steering.linear = steering.linear * Agent.MaxAccel;
            return steering;
        }
    }
}