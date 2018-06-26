using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Agent))]
    public class Chaser : BasicAi
    {
        public Transform MainDestination;

        private void FixedUpdate()
        {
            if (PhotonNetwork.isMasterClient)
            {
                List<SteeringTypes> steerings = Tree.GetActions(this);
                agent.Attacking = false;
                foreach (SteeringTypes st in steerings)
                {
                    switch (st)
                    {
                        case SteeringTypes.ATTACK:
                            Attack();
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
    }
}