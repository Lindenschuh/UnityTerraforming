using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class DecisionTree : DecisionTreeNode
    {
        public DecisionTreeNode Root;
        public Action NewAction;
        private Action OldAction;

        public override DecisionTreeNode MakeDecision()
        {
            return Root.MakeDecision();
        }

        private void Update()
        {
            NewAction.Activated = false;
            OldAction = NewAction;
            NewAction = MakeDecision() as Action;
            if (NewAction == null)
                NewAction = OldAction;
            NewAction.Activated = true;
        }
    }
}