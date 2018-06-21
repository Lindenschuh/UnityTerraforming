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
        public GameObject EntityPrefab;

        // How big will the Waves be.
        public int WaveCount;

        public int MaxEntitiesAtOnce;

        public float WaitBetweenSpawn;
        public float WaitBetweenWaves;
        public float WaitOnStart;

        public readonly UnityEvent OnCapturedEvent;

        private List<GameObject> _entitiesAlive;

        private bool _captured = false;

        private void Start()
        {
            StartCoroutine(SpawnWaves());
            _entitiesAlive = new List<GameObject>();
        }

        public IEnumerator SpawnWaves()
        {
            yield return new WaitForSeconds(WaitOnStart);

            // as long the point is not captured
            while (!_captured)
            {
                if (_entitiesAlive.Count < MaxEntitiesAtOnce)
                    for (int i = 0; i < WaveCount; i++)
                    {
                        if (_captured) break;
                        var spawend = Instantiate(EntityPrefab, transform.transform.position, Quaternion.identity);
                        InstantiateTowerSpecificEnemy(spawend);
                        _entitiesAlive.Add(spawend);
                        yield return new WaitForSeconds(WaitBetweenSpawn);
                    }

                yield return new WaitForSeconds(WaitBetweenWaves);
            }
        }

        public void SpawedInstanceDied(GameObject instance)
        {
            _entitiesAlive.Remove(instance);
            if (_entitiesAlive.Count == 0)
            {
                _captured = true;
                GetComponentInChildren<Crystral>().TooggleCaptured(_captured);
                OnCapturedEvent.Invoke();
            }
        }

        public abstract void InstantiateTowerSpecificEnemy(GameObject spawnedEntity);
    }
}