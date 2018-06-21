using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class EnemyInsideTowerRadiusNode : BehaviourNode
    {
        public override List<SteeringTypes> GetActions(BasicAi ai)
        {
            if ((ai as Guardian).CheckIfInsideOfGuardianDestination())
            {
                return PositiveNode.GetActions(ai);
            }
            return NegativeNode.GetActions(ai);
        }
    }
}