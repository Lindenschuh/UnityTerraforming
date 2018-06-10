using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public interface ISpawnerObserver
    {
        void OnCaptured();
    }

    public abstract class Spawner : MonoBehaviour
    {
        public float WaveIntervall;
        public float SpawnRadius;
        public float WaveCount;
        public float EntityGrowFactor;

        private List<Agent> _entitiesAlive;
        private List<ISpawnerObserver> _observers;

        private bool _captured = false;

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

        public abstract void SpawnWave();

        public void SpawnerCapturedEvent() => _observers.ForEach(o => o.OnCaptured());
    }
}