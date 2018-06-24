using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [System.Serializable]
    public abstract class BehaviourNode
    {
        public BehaviourNode PositiveNode;
        public BehaviourNode NegativeNode;

        public abstract List<SteeringTypes> GetActions(BasicAi ai);
    }
}