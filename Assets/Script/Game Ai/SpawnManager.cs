using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager identity;

        private const float MIN_SPAWN_RATE_SLIDER = 1f;
        private const float MAX_SPAWN_RATE_SLIDER = 5f;

        public Spawner SpawnPrefab;
        public Transform Field;

        public int MaxEntityToSpawn;

        [Range(MIN_SPAWN_RATE_SLIDER, MAX_SPAWN_RATE_SLIDER)]
        public float MinSpawnRate;

        public int MaxWaveCount;
        public float BorderOffset;

        [Range(1f, 2f)]
        public float MaxWaveMultiplier;

        public int MaxSpawner;

        private List<Spawner> _createdSpawner;

        private void Awake()
        {
            if (identity == null)
            {
                identity = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _createdSpawner = new List<Spawner>();

            for (int i = 0; i < MaxSpawner; i++)
            {
                CreateRandomSpawner();
            }
        }

        public void CreateRandomSpawner()
        {
            var scale = Field.localScale * 10 - new Vector3(BorderOffset, 0, BorderOffset);
            var position = new Vector3(Random.value * scale.x, scale.y, Random.value * scale.z);
            position = transform.TransformPoint(position - scale / 2);

            var spawner = Instantiate(SpawnPrefab, position, Quaternion.identity);
            spawner.Init(
                maxEntities: Random.Range(1, MaxEntityToSpawn),
                spawnRate: Random.Range(MinSpawnRate, MAX_SPAWN_RATE_SLIDER),
                waveCount: Random.Range(0, MaxWaveCount),
                waveMultipier: Random.Range(1f, MaxWaveMultiplier)
                );

            _createdSpawner.Add(spawner);
        }
    }
}