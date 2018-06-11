using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Seek))]
    [RequireComponent(typeof(Arrive))]
    [RequireComponent(typeof(Wander))]
    [RequireComponent(typeof(AvoidAgents))]
    [RequireComponent(typeof(AvoidWall))]
    public class Guardian : MonoBehaviour
    {
        public Spawner GuardianDestination;
        public float GuardRadius;
        public float LookRadius;
        public float AttacRadius;

        public Transform PlayerTransform;

        private int _playerLayer;

        public Seek SeekRef;
        public Arrive ArriveRef;

        private void Awake()
        {
            _playerLayer = LayerMask.NameToLayer("Player");

            if (GuardianDestination != null)
                ArriveRef.Target = GuardianDestination.gameObject;
        }

        public bool PlayerInAttackRange()
        {
            if (PlayerTransform == null) return false;

            return (transform.position - PlayerTransform.position).magnitude <= AttacRadius;
        }

        public bool InsideGuardRadius()
        {
            if (GuardianDestination == null) return false;
            return (transform.position - GuardianDestination.transform.position).magnitude < GuardRadius;
        }

        public bool CheckPlayerInSight()
        {
            Collider[] coliders = Physics.OverlapSphere(transform.position, LookRadius);
            foreach (Collider col in coliders)
            {
                if (col.gameObject.layer == _playerLayer)
                {
                    PlayerTransform = col.gameObject.transform;
                    SeekRef.Target = PlayerTransform.gameObject;
                    return true;
                }
            }
            return false;
        }
    }
}