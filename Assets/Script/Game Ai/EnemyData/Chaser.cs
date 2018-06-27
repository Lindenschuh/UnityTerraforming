using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Agent))]
    public class Chaser : BasicAi
    {
        public float AvoidWallForce = 100;

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
                            if (_nextAttack < Time.time)
                            {
                                _nextAttack = Time.time + AttackRate;
                                Attack();
                            }
                            break;

                        case SteeringTypes.SEEK:
                            agent.SetSteering(SteeringManager.GetSeek(agent, (_target != null) ? _target.transform.position : MainDestination.transform.position));
                            break;

                        case SteeringTypes.FLEE:
                            break;

                        case SteeringTypes.ARRIVE:
                            agent.SetSteering(SteeringManager.GetArrive(agent, MainDestination.transform.position, TargetRadius, SlowRadius));
                            break;

                        case SteeringTypes.LEAVE:
                            break;

                        case SteeringTypes.AVOID_WALLS:
                            if (_nextavoid < Time.time)
                            {
                                _nextavoid = Time.time + WallAvoidanceRate;
                                _avoidanceTarget = null;
                            }
                            agent.SetSteering(SteeringManager.GetAvoidWalls(agent, LookRadius, AvoidDistance, ref _avoidanceTarget, EnvironmentLayers, FeelerAngle, FeelerScale), AvoidWallForce);

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