using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class Guardian : BasicAi
    {
        public Transform GuardianDestination;
        public float GuardRadius;

        public bool CheckIfInsideOfGuardianDestination() => (transform.position - GuardianDestination.position).magnitude <= GuardRadius;
    }
}