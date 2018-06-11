using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public abstract class DecisionTreeNode : MonoBehaviour
    {
        public abstract DecisionTreeNode MakeDecision();
    }
}