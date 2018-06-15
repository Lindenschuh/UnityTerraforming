using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class GuardianSpawner : Spawner
    {
        public override void InitialiseTowerSpecificEnemy(GameObject spawnedEntity)
        {
            var guardian = spawnedEntity.GetComponent<Guardian>();

            if (guardian != null)
            {
                guardian.GuardianDestination = transform;
            }
            else
                throw new MissingComponentException("In the Spawned Prefab is no Guardian Script attached. Maybe you choose the wrong Prefab.");
        }
    }
}