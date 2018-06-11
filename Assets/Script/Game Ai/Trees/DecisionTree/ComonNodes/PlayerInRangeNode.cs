using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class PlayerInRangeNode : DecisionTreeNode
    {
        public Action TrueAction;
        public Action FalseAction;

        public Guardian Guardian;

        public override DecisionTreeNode MakeDecision()
        {
            if (Guardian.PlayerInAttackRange())
            {
                return TrueAction.MakeDecision();
            }
            return FalseAction.MakeDecision();
        }
    }
}