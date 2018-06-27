using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class GuardianTree : BehaviourTree
    {
        public override void InitialiseTree()
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
    }
}