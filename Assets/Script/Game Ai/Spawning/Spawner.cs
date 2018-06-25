using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace UnityTerraforming.GameAi
{
    public abstract class Spawner : Photon.PunBehaviour
    {
        public GameObject GuardianPrefab;
        public GameObject AttackerPrefab;

        // How big will the Waves be.
        public int GuardianWaveCount;

        public int AttackingWaveCount;

        public int MaxGuardianWavesAlive;
        public int MaxAttackingWavesAlive;

        public float WaitBetweenSpawn;
        public float WaitBetweenWaves;
        public float WaitOnStart;

        public float AttackerOffset;

        public readonly UnityEvent OnCapturedEvent;

        private List<GameObject> _guardingEntitiesAlive;

        private List<GameObject> _attackingEntitiesAlive;

        private bool _captured = false;

        private void Start()
        {
            StartCoroutine(SpawnGuardingWaves());
            StartCoroutine(SpawnAttackingWaves());
            _guardingEntitiesAlive = new List<GameObject>();
            _attackingEntitiesAlive = new List<GameObject>();
        }

        public IEnumerator SpawnGuardingWaves()
        {
            yield return new WaitForSeconds(WaitOnStart);

            // as long the point is not captured
            while (!_captured)
            {
                if ((_guardingEntitiesAlive.Count / GuardianWaveCount) < MaxGuardianWavesAlive)
                    for (int i = 0; i < GuardianWaveCount; i++)
                    {
                        if (_captured) break;
                        var spawend = Instantiate(GuardianPrefab, transform.transform.position, Quaternion.identity);
                        InstantiateTowerSpecificGuard(spawend);
                        _guardingEntitiesAlive.Add(spawend);
                        yield return new WaitForSeconds(WaitBetweenSpawn);
                    }

                yield return new WaitForSeconds(WaitBetweenWaves);
            }
        }

        public IEnumerator SpawnAttackingWaves()
        {
            yield return new WaitForSeconds(WaitOnStart + AttackerOffset);

            // as long the point is not captured
            while (!_captured)
            {
                if ((_attackingEntitiesAlive.Count / GuardianWaveCount) < MaxGuardianWavesAlive)
                    for (int i = 0; i < GuardianWaveCount; i++)
                    {
                        if (_captured) break;
                        var spawend = Instantiate(AttackerPrefab, transform.transform.position, Quaternion.identity);
                        InstantiateTowerSpecificAtttacker(spawend);
                        _attackingEntitiesAlive.Add(spawend);
                        yield return new WaitForSeconds(WaitBetweenSpawn + AttackerOffset);
                    }

                yield return new WaitForSeconds(WaitBetweenWaves + AttackerOffset);
            }
        }

        public void SpawedInstanceDied(GameObject instance)
        {
            if (_guardingEntitiesAlive.Contains(instance))
                _guardingEntitiesAlive.Remove(instance);
            if (_attackingEntitiesAlive.Contains(instance))
                _attackingEntitiesAlive.Remove(instance);
            if (_guardingEntitiesAlive.Count == 0)
            {
                _captured = true;
                GetComponentInChildren<Crystral>().TooggleCaptured(_captured);
                OnCapturedEvent.Invoke();
            }
        }

        public abstract void InstantiateTowerSpecificGuard(GameObject spawnedEntity);

        public abstract void InstantiateTowerSpecificAtttacker(GameObject spawedEntity);
    }
}