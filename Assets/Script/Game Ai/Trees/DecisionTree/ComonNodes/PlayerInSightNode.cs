using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class PlayerInSightNode : DecisionTreeNode
    {
        public DecisionTreeNode TrueNode;
        public DecisionTreeNode FalseNode;

        public Guardian Guardian;

        public override DecisionTreeNode MakeDecision()
        {
            if (Guardian.CheckPlayerInSight())
            {
                return TrueNode.MakeDecision();
            }
            return FalseNode.MakeDecision();
        }
    }
}