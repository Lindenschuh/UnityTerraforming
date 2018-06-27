using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class GuardianSpawner : Spawner
    {
        public override void InstantiateTowerSpecificAtttacker(GameObject spawedEntity)
        {
            var chaser = spawedEntity.GetComponent<Chaser>();
            if (chaser != null)
            {
                chaser.Spawner = this;
                chaser.Player = Player;
                chaser.MainDestination = MainDestination;
            }
            else
            {
                Debug.LogError("In the Spawned Prefab is no Chaser Script attached. Maybe you choose the wrong Prefab.");
            }
        }

        public override void InstantiateTowerSpecificGuard(GameObject spawnedEntity)
        {
            var guardian = spawnedEntity.GetComponent<Guardian>();

            if (guardian != null)
            {
                guardian.Spawner = this;
                guardian.MainDestination = MainDestination;
                guardian.Player = Player;
            }
            else
            {
                Debug.LogError("In the Spawned Prefab is no Guardian Script attached. Maybe you choose the wrong Prefab.");
            }
        }
    }
}