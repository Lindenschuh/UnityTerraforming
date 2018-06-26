using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class GuardianSpawner : Spawner
    {
        public Transform MainDestination;

        public override void InstantiateTowerSpecificAtttacker(GameObject spawedEntity)
        {
            var chaser = spawedEntity.GetComponent<Chaser>();
            if (chaser != null)
            {
                chaser.Spawner = this;
                chaser.MainDestination = MainDestination;
            }
            else
            {
                Debug.Log("In the Spawned Prefab is no Chaser Script attached. Maybe you choose the wrong Prefab.");
            }
        }

        public override void InstantiateTowerSpecificGuard(GameObject spawnedEntity)
        {
            var guardian = spawnedEntity.GetComponent<Guardian>();

            if (guardian != null)
            {
                guardian.Spawner = this;
            }
            else
            {
                Debug.Log("In the Spawned Prefab is no Guardian Script attached. Maybe you choose the wrong Prefab.");
            }
        }
    }
}