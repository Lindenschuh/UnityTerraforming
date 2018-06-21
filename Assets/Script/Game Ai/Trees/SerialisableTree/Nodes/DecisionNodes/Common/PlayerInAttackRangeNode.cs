using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class PlayerInAttackRangeNode : BehaviourNode
    {
        public override List<SteeringTypes> GetActions(BasicAi ai)
            => (ai.CheckPlayerInAttackRange()) ? PositiveNode.GetActions(ai) : NegativeNode.GetActions(ai);
    }
}