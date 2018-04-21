using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private const float MIN_SPAWN_RATE_SLIDER = 1f;
    private const float MAX_SPAWN_RATE_SLIDER = 5f;

    public GameEntity Prefab;
    public int MaxEntities;

    [Range(MIN_SPAWN_RATE_SLIDER, MAX_SPAWN_RATE_SLIDER)]
    public float SpawnRate;

    // If WaveCount is 0 then there is no Wave Limit
    public int WaveCount;

    public float WaveMultiplier;

    public GameEntity Destination;

    private float _nextSpawnTime;
    private int _entitiyCount;
    private bool pausedSpawning;

    public void Init(GameEntity prefab, GameEntity destination, int maxEntities = 10, float spawnRate = .5f, int waveCount = 0, float waveMultipier = 1f)
    {
        Prefab = prefab;
        MaxEntities = maxEntities;
        SpawnRate = spawnRate;
        WaveCount = waveCount;
        WaveMultiplier = waveMultipier;
        Destination = destination;
    }

    private void FixedUpdate()
    {
        if (!pausedSpawning)
        {
            SpawnEntity();
        }

        Debug.Log(_entitiyCount);
        Debug.Log(pausedSpawning);
    }

    private void SpawnEntity()
    {
        if (Time.time > _nextSpawnTime && _entitiyCount < MaxEntities)
        {
            _nextSpawnTime = Time.time + SpawnRate;
            var entity = Instantiate(Prefab, new Vector3(transform.position.x, Prefab.transform.localScale.y / 2, transform.position.z), Quaternion.identity);
            entity.Destination = Destination;
            entity.Spawner = this;
            _entitiyCount++;
            if (_entitiyCount == MaxEntities)
            {
                pausedSpawning = true;
            }
        }
    }

    public void DestroyChild(GameEntity entity)
    {
        Destroy(entity.gameObject);
        _entitiyCount--;
        if (_entitiyCount <= 0)
        {
            pausedSpawning = false;
        }
    }
}