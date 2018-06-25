using UnityEngine;
using System.Collections.Generic;

namespace UnityTerraforming.GameAi
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        public BehaviourNode RootNode;

        private void Awake()
        {
            InitialiseTree();
        }

        public List<SteeringTypes> GetActions(BasicAi ai)
        {
            var types = RootNode.GetActions(ai);

            return types;
        }

        public abstract void InitialiseTree();
    }
}