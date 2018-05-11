using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{        
        public GameObject[] prefabs;

    void Start()
    {
        SpawnObjects(prefabs, 100, 0, 100, 0, 100);
    }

    private void SpawnObjects(GameObject[] prefabsToSpawn, int spawnCount, float minX, float maxX, float minY, float maxY, int maximumRetry = 200)
    {
        foreach (GameObject prefab in prefabsToSpawn)
        {
            StartCoroutine(SpawnObject(prefab, Mathf.RoundToInt(spawnCount / prefabsToSpawn.Length), minX, maxX, minY, maxY, maximumRetry));
        }
    }

    IEnumerator SpawnObject(GameObject prefabToSpawn, int spawnCount, float minX, float maxX, float minY, float maxY, int maximumRetry = 200)
    {
        int currentObjectsCount = 0;
        int currentRetry = 0;
        Vector3 randomPosition;

        CheckForCollisions template = Instantiate(prefabToSpawn).AddComponent<CheckForCollisions>();
        PositionCheck positionCheck = new PositionCheck();


        while (currentObjectsCount < spawnCount && currentRetry < maximumRetry)
        {
            positionCheck.Reset();
            positionCheck.PossitionAllowed();
            randomPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minY, maxY));
            positionCheck.randomPosition = randomPosition;

            yield return StartCoroutine(template.IsColliding(positionCheck));

            if (!positionCheck.isPositionAvailable || positionCheck.isColliding)
            {
                currentRetry++;
                continue;
            }

            Instantiate(prefabToSpawn, positionCheck.randomPosition, positionCheck.normalizedRotation);
            currentObjectsCount++;
        }
    }
}
