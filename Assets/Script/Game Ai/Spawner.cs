﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class Spawner : MonoBehaviour
    {
        private const float MIN_SPAWN_RATE_SLIDER = 1f;
        private const float MAX_SPAWN_RATE_SLIDER = 5f;

        public int MaxEntities;

        [Range(MIN_SPAWN_RATE_SLIDER, MAX_SPAWN_RATE_SLIDER)]
        public float SpawnRate;

        // If WaveCount is 0 then there is no Wave Limit
        public int WaveCount;

        public float WaveMultiplier;

        private float _nextSpawnTime;
        private int _entitiyCount;
        private bool pausedSpawning;

        public void Init(int maxEntities = 10, float spawnRate = .5f, int waveCount = 0, float waveMultipier = 1f)
        {
            MaxEntities = maxEntities;
            SpawnRate = spawnRate;
            WaveCount = waveCount;
            WaveMultiplier = waveMultipier;
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (!pausedSpawning)
                {
                    SpawnEntity();
                }
            }
        }

        private void SpawnEntity()
        {
            if (Time.time > _nextSpawnTime && _entitiyCount < MaxEntities)
            {
                _nextSpawnTime = Time.time + SpawnRate;
                _entitiyCount++;
                if (_entitiyCount == MaxEntities)
                {
                    pausedSpawning = true;
                }
            }
        }
    }
}