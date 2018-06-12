using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityTerraforming.GameAi
{
    public interface ISpawnerObserver
    {
        void OnCaptured();
    }

    public abstract class Spawner : Photon.PunBehaviour
    {
        public GameObject EntityPrefab;

        // In Which Radius will the Enemies Spawn
        public float SpawnRadius;

        // How big will the Waves be.
        public int WaveCount;

        // How big should the Wave be the next time
        public float EntityGrowFactor;

        public int MaxEntitiesAtOnce;

        public float DistanceBetweenEntities;

        public float WaitBetweenSpawn;
        public float WaitBetweenWaves;
        public float WaitOnStart;

        private List<GameObject> _entitiesAlive;
        private List<ISpawnerObserver> _observers;

        private bool _captured = false;
        private int _currentWaveCounter;

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
                int enemiesToSpawn = Math.Min(MaxEntitiesAtOnce, WaveCount - _currentWaveCounter);
                var pointsForSpawn = new List<Vector3>();

                // initialise all points where enemies will spawn
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    var point = transform.position;
                    if (IsPointvalid(point, pointsForSpawn))
                    {
                        pointsForSpawn.Add(point);
                    }
                }

                // Spawn the Entities
                pointsForSpawn.ForEach(point =>
                {
                    var spawned = PhotonNetwork.Instantiate(EntityPrefab.name, point, Quaternion.identity, 0);
                    InitialiseTowerSpecificEnemy(spawned);
                    _entitiesAlive.Add(spawned);
                });

                if (WaveCount < _currentWaveCounter)
                {
                    yield return new WaitForSeconds(WaitBetweenWaves);
                    _currentWaveCounter = 0;
                }
                else
                {
                    yield return new WaitForSeconds(WaitBetweenSpawn);
                }
            }
        }

        private bool IsPointvalid(Vector3 point, List<Vector3> setPoints)
        {
            List<Vector3> agentPoints = (from ag in _entitiesAlive
                                         select ag.transform.position).ToList();

            return !(PointInRange(point, setPoints) || PointInRange(point, agentPoints));
        }

        public bool PointInRange(Vector3 point, List<Vector3> positions)
        {
            foreach (Vector3 pos in positions)
            {
                if ((pos - point).magnitude < DistanceBetweenEntities)
                    return true;
            }
            return false;
        }

        public abstract void InitialiseTowerSpecificEnemy(GameObject spawnedEntity);

        public void SpawnerCapturedEvent() => _observers.ForEach(o => o.OnCaptured());

        #region Observer

        /// <summary>
        /// Subscribe an observer which has implemented the ISpawnerObserver interface.
        /// </summary>
        /// <param name="observer">The observer which is subscribing</param>
        /// <returns>Return true if the Observer could be added and is not already in the list of observers.</returns>
        public bool Subscribe(ISpawnerObserver observer)
        {
            if (_observers.Contains(observer))
                return false;

            _observers.Add(observer);
            return true;
        }

        /// <summary>
        /// Unsubscribe from the list of observers.
        /// </summary>
        /// <param name="observer">The observer which is unsubscribing.</param>
        /// <returns>Returns true if the observer was in the list of ovservers.</returns>
        public bool Unsubscribe(ISpawnerObserver observer)
        {
            if (!_observers.Contains(observer))
                return false;

            _observers.Remove(observer);
            return true;
        }

        #endregion Observer
    }
}