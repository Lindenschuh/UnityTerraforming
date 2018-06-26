using UnityEngine;
using System.Collections.Generic;
using System;
using Photon;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Agent))]
    public class Guardian : BasicAi
    {
        public GuardianSpawner GuardianDestination;
        public float GuardRadius;

        public BehaviourTree Tree;

        public float SlowRadius = 15;
        public float TargetRadius = 0.1f;
        public float AvoidDistance = 10;

        public float FeelerAngle = 60f;
        public float FeelerScale = 2f;

        public float CollisionRadius = 4f;

        public float WanderOffset;
        public float WanderRadius;
        public float WanderRate;

        private Agent agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.isMasterClient)
            {
                List<SteeringTypes> steerings = Tree.GetActions(this);
                foreach (SteeringTypes st in steerings)
                {
                    switch (st)
                    {
                        case SteeringTypes.ATTACK:
                            AttackPlayer();
                            break;

                        case SteeringTypes.SEEK:
                            agent.SetSteering(SteeringManager.GetSeek(agent, LastPlayerPosition.position));
                            break;

                        case SteeringTypes.FLEE:
                            break;

                        case SteeringTypes.ARRIVE:
                            agent.SetSteering(SteeringManager.GetArrive(agent, GuardianDestination.transform.position, TargetRadius, SlowRadius));
                            break;

                        case SteeringTypes.LEAVE:
                            break;

                        case SteeringTypes.AVOID_WALLS:
                            agent.SetSteering(SteeringManager.GetAvoidWalls(agent, LookRadius, AvoidDistance, EnvironmentLayers, FeelerAngle, FeelerScale));
                            break;

                        case SteeringTypes.AVOID_AGENTS:
                            agent.SetSteering(SteeringManager.GetAvoidAgents(agent, CheckSourroundingAgents(), CollisionRadius));
                            break;

                        case SteeringTypes.WANDER:
                            agent.SetSteering(SteeringManager.GetWander(agent, WanderOffset, WanderRadius, WanderRate));
                            break;
                    }
                }
            }
        }

        public bool CheckIfInsideOfGuardianDestination() => (transform.position - GuardianDestination.transform.position).magnitude <= GuardRadius;

        private void AttackPlayer()
        {
            Debug.Log("I WILL KILL YOU! MORTAL SCUMBAG!");
        }

        private void OnDestroy()
        {
            GuardianDestination.SpawedInstanceDied(gameObject);
        }
    }
}