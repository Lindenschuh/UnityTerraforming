using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class WanderAroundAction : BehaviourNode
    {
        public override List<SteeringTypes> GetActions(BasicAi ai)
            => new List<SteeringTypes>()
            {
                SteeringTypes.WANDER,
                SteeringTypes.AVOID_AGENTS,
                SteeringTypes.AVOID_WALLS
            };
    }
}