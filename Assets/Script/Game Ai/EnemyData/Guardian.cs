using UnityEngine;
using System.Collections.Generic;
using System;
using Photon;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Agent))]
    public class Guardian : BasicAi
    {
        public Transform GuardianDestination;
        public float GuardRadius;

        public BehaviourTree Tree;

        public Transform Player;

        public float SlowRadius;
        public float TargetRadius;
        public float AvoidDistance;

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

        public bool CheckIfInsideOfGuardianDestination() => (transform.position - GuardianDestination.position).magnitude <= GuardRadius;

        private void Update()
        {
            //if (PhotonNetwork.isMasterClient)
            //{
            List<SteeringTypes> steerings = Tree.GetActions(this);
            foreach (SteeringTypes st in steerings)
            {
                switch (st)
                {
                    case SteeringTypes.ATTACK:
                        AttackPlayer();
                        break;

                    case SteeringTypes.SEEK:
                        agent.SetSteering(SteeringManager.GetSeek(agent, Player.position));
                        break;

                    case SteeringTypes.FLEE:
                        break;

                    case SteeringTypes.ARRIVE:
                        agent.SetSteering(SteeringManager.GetArrive(agent, GuardianDestination.position, TargetRadius, SlowRadius));
                        break;

                    case SteeringTypes.LEAVE:
                        break;

                    case SteeringTypes.AVOID_WALLS:
                        agent.SetSteering(SteeringManager.GetAvoidWalls(agent, LookRadius, AvoidDistance, FeelerAngle, FeelerScale));
                        break;

                    case SteeringTypes.AVOID_AGENTS:
                        agent.SetSteering(SteeringManager.GetAvoidAgents(agent, CheckSourroundingAgents(), CollisionRadius));
                        break;

                    case SteeringTypes.WANDER:
                        agent.SetSteering(SteeringManager.GetWander(agent, WanderOffset, WanderRadius, WanderRate));
                        break;
                }
            }
            //}
        }

        private List<Agent> CheckSourroundingAgents()
        {
            var targets = new List<Agent>();
            foreach (Collider c in Physics.OverlapSphere(transform.position, LookRadius))
            {
                var agent = c.GetComponent<Agent>();
                if (agent != null)
                    targets.Add(agent);
            }
            return targets;
        }

        private void AttackPlayer()
        {
            Debug.Log("I WILL KILL YOU! MORTAL SCUMBAG!");
        }
    }
}