using UnityEngine;
using System.Collections.Generic;
using System;
using Photon;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Agent))]
    public class Guardian : BasicAi
    {
        public float GuardRadius = 75f;

        public float WanderOffset = 14f;
        public float WanderRadius = 5f;
        public float WanderRate = 180f;

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
                            agent.SetSteering(SteeringManager.GetSeek(agent, (_target != null) ? _target.transform.position : Spawner.transform.position));
                            break;

                        case SteeringTypes.FLEE:
                            break;

                        case SteeringTypes.ARRIVE:
                            agent.SetSteering(SteeringManager.GetArrive(agent, Spawner.transform.position, TargetRadius, SlowRadius));
                            break;

                        case SteeringTypes.LEAVE:
                            break;

                        case SteeringTypes.AVOID_WALLS:
                            if (_nextavoid < Time.time)
                            {
                                _nextavoid = Time.time + WallAvoidanceRate;
                                _avoidanceTarget = null;
                            }
                            agent.SetSteering(SteeringManager.GetAvoidWalls(agent, LookRadius, AvoidDistance, ref _avoidanceTarget, EnvironmentLayers, FeelerAngle, FeelerScale));
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

        public bool CheckIfInsideOfGuardianDestination() => (transform.position - Spawner.transform.position).magnitude <= GuardRadius;
    }
}