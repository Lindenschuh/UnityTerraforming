using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public abstract class BasicAi : Photon.MonoBehaviour
    {
        public GuardianSpawner Spawner;

        public float LookRadius = 15f;
        public float AttackRadius = 2f;

        public BehaviourTree Tree;

        public float SlowRadius = 15f;
        public float TargetRadius = 0.1f;
        public float AvoidDistance = 10f;

        public float FeelerAngle = 60f;
        public float FeelerScale = 2f;

        public float CollisionRadius = 4f;

        public float AnimationDuration = 2f;

        public LayerMask PlayerLayer;
        public LayerMask EnemyLayers;
        public LayerMask EnvironmentLayers;

        [HideInInspector]
        public Transform LastPlayerPosition;

        protected Agent agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
        }

        public bool CheckPlayerInSight()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, LookRadius, PlayerLayer);

            if (colliders.Length > 0)
            {
                LastPlayerPosition = colliders[0].transform;
                return true;
            }
            return false;
        }

        public bool CheckPlayerInAttackRange() => Physics.OverlapSphere(transform.position, AttackRadius, PlayerLayer).Length > 0;

        protected List<Agent> CheckSourroundingAgents()
        {
            var targets = new List<Agent>();
            foreach (Collider c in Physics.OverlapSphere(transform.position, LookRadius, EnemyLayers))
            {
                var agent = c.GetComponent<Agent>();
                if (agent != null)
                    targets.Add(agent);
            }
            return targets;
        }

        protected void Attack()
        {
            Debug.Log("I WILL KILL YOU! MORTAL SCUMBAG!");
            agent.Attacking = true;
            if (LastPlayerPosition != null)
            {
                LastPlayerPosition.GetComponent<Health>().AddDamage(agent.Damage);
            }
        }

        public void InstanceDied()
        {
            Spawner.SpawedInstanceDied(gameObject);
            agent.Dying = true;
            GetComponent<Drops>().Drop();
            new WaitForSeconds(AnimationDuration);
            Destroy(gameObject);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}