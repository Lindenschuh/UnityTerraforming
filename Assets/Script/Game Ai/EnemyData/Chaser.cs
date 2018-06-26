using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Agent))]
    public class Chaser : BasicAi
    {
        public GuardianSpawner spawner;

        public BehaviourTree Tree;

        public float SlowRadius = 15;
        public float TargetRadius = 0.1f;
        public float AvoidDistance = 10;

        public float FeelerAngle = 60f;
        public float FeelerScale = 2f;

        public float CollisionRadius = 4f;

        public Transform MainDestination;

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
                            //Attack();
                            break;

                        case SteeringTypes.SEEK:
                            agent.SetSteering(SteeringManager.GetSeek(agent, LastPlayerPosition.position));
                            break;

                        case SteeringTypes.FLEE:
                            break;

                        case SteeringTypes.ARRIVE:
                            agent.SetSteering(SteeringManager.GetArrive(agent, MainDestination.transform.position, TargetRadius, SlowRadius));
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
                            break;
                    }
                }
            }
        }

        private void Attack()
        {
            Debug.Log("I WILL KILL YOU! MORTAL SCUMBAG!");
            if (LastPlayerPosition != null)
            {
                LastPlayerPosition.GetComponent<Health>().AddDamage(agent.Damage);
            }
        }

        private void OnDestroy()
        {
            spawner.SpawedInstanceDied(gameObject);
        }
    }
}