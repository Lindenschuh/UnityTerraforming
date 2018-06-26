using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class BasicAi : Photon.MonoBehaviour
    {
        public float LookRadius;
        public float AttackRadius;

        public LayerMask PlayerLayer;
        public LayerMask EnemyLayers;
        public LayerMask EnvironmentLayers;

        [HideInInspector]
        public Transform LastPlayerPosition;

        private void Awake()
        {
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
    }
}