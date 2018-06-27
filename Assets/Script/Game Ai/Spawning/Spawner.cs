using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Photon;

namespace UnityTerraforming.GameAi
{
    public class SpawnerStats
    {
        public int SpawnedTogether { get; private set; }
        public int CurrentAlive { get; private set; }
        public float TimeTilCaptured { get; private set; }
        public float SpawnTime { get; private set; }

        public SpawnerStats()
        {
            SpawnTime = Time.time;
        }

        public void Spawend()
        {
            SpawnedTogether++;
            CurrentAlive++;
        }

        public void Died() => CurrentAlive--;

        public void Captured() => TimeTilCaptured = Time.time - SpawnTime;
    }

    public abstract class Spawner : Photon.PunBehaviour
    {
        public Transform Player;
        public Transform MainDestination;

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

        public Transform SpawnPoint;

        public readonly UnityEvent OnCapturedEvent;

        public List<GameObject> GuardingEntitiesAlive { get; private set; }

        public List<GameObject> AttackingEntitiesAlive { get; private set; }

        private bool _captured = false;

        public SpawnerStats Stats;

        private void Awake()
        {
            Stats = new SpawnerStats();
        }

        private void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                StartCoroutine(SpawnGuardingWaves());
                StartCoroutine(SpawnAttackingWaves());
                GuardingEntitiesAlive = new List<GameObject>();
                AttackingEntitiesAlive = new List<GameObject>();
                if (Player == null)
                    Player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
            }
        }

        public IEnumerator SpawnGuardingWaves()
        {
            yield return new WaitForSeconds(WaitOnStart);

            // as long the point is not captured
            while (!_captured)
            {
                if ((GuardingEntitiesAlive.Count / GuardianWaveCount) < MaxGuardianWavesAlive)
                    for (int i = 0; i < GuardianWaveCount; i++)
                    {
                        if (_captured) break;
                        var spawend = PhotonNetwork.Instantiate(GuardianPrefab.name, SpawnPoint.position, Quaternion.identity, 0);
                        InstantiateTowerSpecificGuard(spawend);
                        GuardingEntitiesAlive.Add(spawend);
                        Stats.Spawend();
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
                if ((AttackingEntitiesAlive.Count / GuardianWaveCount) < MaxGuardianWavesAlive)
                    for (int i = 0; i < GuardianWaveCount; i++)
                    {
                        if (_captured) break;
                        var spawend = PhotonNetwork.Instantiate(AttackerPrefab.name, SpawnPoint.position, Quaternion.identity, 0);
                        InstantiateTowerSpecificAtttacker(spawend);
                        AttackingEntitiesAlive.Add(spawend);
                        Stats.Spawend();
                        yield return new WaitForSeconds(WaitBetweenSpawn + AttackerOffset);
                    }

                yield return new WaitForSeconds(WaitBetweenWaves + AttackerOffset);
            }
        }

        public void SpawedInstanceDied(GameObject instance)
        {
            if (GuardingEntitiesAlive.Contains(instance))
            {
                GuardingEntitiesAlive.Remove(instance);
                Stats.Died();
            }
            if (AttackingEntitiesAlive.Contains(instance))
            {
                AttackingEntitiesAlive.Remove(instance);
                Stats.Died();
            }
            if (GuardingEntitiesAlive.Count == 0 && !_captured)
            {
                GetComponent<GuardianSpawner>()._captured = true;
                GetComponentInChildren<Crystral>().TooggleCaptured(_captured);
                OnCapturedEvent.Invoke();
                Stats.Captured();
            }
        }

        public abstract void InstantiateTowerSpecificGuard(GameObject spawnedEntity);

        public abstract void InstantiateTowerSpecificAtttacker(GameObject spawedEntity);

    }
}