using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public abstract class BasicAi : Photon.PunBehaviour
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

        public float AttackRate = .5f;
        public float AnimationDuration = 2f;
        public float WaitBetweenScans = 3f;
        public float WallAvoidanceRate = 0.5f;

        public LayerMask PlayerLayer;
        public LayerMask EnemyLayers;
        public LayerMask EnvironmentLayers;

        public Transform Player;
        public Transform MainDestination;

        protected GameObject _target;

        protected Agent agent;
        protected Steering WallSteering;

        protected List<Agent> _souroundingAgents;

        private float _nextScan = 0f;
        protected float _nextavoid = 0f;
        protected float _nextAttack = 0f;

        protected Vector3? _avoidanceTarget;

        private void Awake()
        {
            agent = GetComponent<Agent>();
            _souroundingAgents = new List<Agent>();
            WallSteering = new Steering();
        }

        public bool CheckPlayerInSight()
        {
            if (MainDestination == null || Player == null)
            {
                Debug.LogError("No Player or Main Destiantion assinged");
                return false;
            }

            if ((transform.position - MainDestination.position).magnitude < LookRadius)
            {
                _target = MainDestination.gameObject;
                return true;
            }
            else if ((transform.position - Player.position).magnitude < LookRadius)
            {
                _target = Player.gameObject;
                return true;
            }
            _target = null;
            return false;
        }

        public bool CheckPlayerInAttackRange() =>
             _target != null && (_target.transform.position - transform.position).magnitude < AttackRadius;

        protected List<Agent> CheckSourroundingAgents()
        {
            if (_nextScan > Time.time)
            {
                var nextScan = Time.time + WaitBetweenScans;
                var targets = new List<Agent>();
                foreach (Collider c in Physics.OverlapSphere(transform.position, LookRadius, EnemyLayers))
                {
                    var agent = c.GetComponent<Agent>();
                    if (agent != null)
                        targets.Add(agent);
                }

                _souroundingAgents = targets;
            }
            return _souroundingAgents;
        }

        protected void Attack()
        {
            agent.Attacking = true;
            if (_target != null)
            {
                _target.GetComponent<Health>().EnemyAddDamage(agent.Damage);
            }
        }

        public void InstanceDied()
        {
            Spawner.SpawedInstanceDied(gameObject);
            agent.Dying = true;
            GetComponent<Drops>().Drop();
            new WaitForSeconds(AnimationDuration);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}