using UnityEngine;
using System.Collections.Generic;

namespace UnityTerraforming.GameAi
{
    public class BehaviourTree : MonoBehaviour
    {
        public BehaviourNode RootNode;

        private void Awake()
        {
            RootNode = new PlayerInEnemySightNode()
            {
                PositiveNode = new PlayerInAttackRangeNode()
                {
                    PositiveNode = new AttackPlayerAction(),
                    NegativeNode = new ChasePlayerAction()
                },
                NegativeNode = new EnemyInsideTowerRadiusNode()
                {
                    PositiveNode = new WanderAroundAction(),
                    NegativeNode = new GoBackToTowerAction()
                }
            };
        }

        public List<SteeringTypes> GetActions(BasicAi ai)
        {
            var types = RootNode.GetActions(ai);

            return types;
        }
    }
}