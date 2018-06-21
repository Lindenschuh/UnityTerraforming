using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class PlayerInEnemySightNode : BehaviourNode
    {
        public override List<SteeringTypes> GetActions(BasicAi ai)
            => (ai.CheckPlayerInSight()) ? PositiveNode.GetActions(ai) : NegativeNode.GetActions(ai);
    }
}