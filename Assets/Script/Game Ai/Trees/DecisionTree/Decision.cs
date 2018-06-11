using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public abstract class Decision : DecisionTreeNode
    {
        public Action nodeTrue;
        public Action nodeFalse;

        public abstract Action GetBranch();
    }
}