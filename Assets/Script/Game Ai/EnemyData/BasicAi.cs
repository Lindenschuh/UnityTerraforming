using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class BasicAi : MonoBehaviour
    {
        public float LookRadius;
        public float AttackRadius;

        public LayerMask PlayerLayer;

        public bool CheckPlayerInSight() => Physics.OverlapSphere(transform.position, LookRadius, PlayerLayer).Length > 0;

        public bool CheckPlayerInAttackRange() => Physics.OverlapSphere(transform.position, AttackRadius, PlayerLayer).Length > 0;
    }
}