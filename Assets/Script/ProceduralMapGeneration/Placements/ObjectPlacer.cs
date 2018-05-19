using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : Photon.PunBehaviour
{        
    public GameObject[] prefabs;
    public int tries;
    public int prefabsCount;
    public Terrain terrain;

    void Start()
    {
        int seed = 0;
        if (PhotonNetwork.isMasterClient)
            seed = Random.Range(0, 100000);
        GetComponent<MapGenerator>().GenerateMap(seed);
        if (PhotonNetwork.isMasterClient)
            StartCoroutine(SpawnObjects(prefabs, RandomizeOnTerrain, prefabsCount, tries));
    }

    private IEnumerator SpawnObjects(GameObject[] prefabsToSpawn, System.Action<PositionCheck> randomizeDelegate, int spawnCount, int maximumRetry = 50)
    {
        foreach (GameObject prefab in prefabsToSpawn)
        {
            PositionCheck positionCheck = new PositionCheck();
            yield return StartCoroutine(SpawnObject(prefab, positionCheck, randomizeDelegate, Mathf.RoundToInt(spawnCount / prefabsToSpawn.Length), maximumRetry));
        }
    }

    IEnumerator SpawnObject(GameObject prefabToSpawn, PositionCheck positionCheck, System.Action<PositionCheck> randomizeDelegate, int spawnCount, int maximumRetry = 50, bool allowOverdraw = false)
    {
        int currentObjectsCount = 0;
        int currentRetry = 0;

        if (prefabToSpawn.GetComponent<Collider>() == null)
            allowOverdraw = true;

        CheckForCollisions template = Instantiate(prefabToSpawn).AddComponent<CheckForCollisions>();


        while (currentObjectsCount < spawnCount && currentRetry < maximumRetry)
        {
            positionCheck.Reset();
            RandomizeOnTerrain(positionCheck);
            if (!allowOverdraw)
                yield return StartCoroutine(template.IsColliding(positionCheck));
            else
                positionCheck.PossitionAllowed();

            if (positionCheck.isColliding)
            {
                currentRetry++;
                continue;
            }
            PhotonNetwork.Instantiate(prefabToSpawn.name, positionCheck.randomPosition, positionCheck.normalizedRotation, 0);
            currentObjectsCount++;
        }
        Destroy(template.gameObject);
    }

    public void RandomizeOnTerrain(PositionCheck positionCheck)
    {
        positionCheck.randomPosition = AlignToTerrain(terrain, 50, 450, 50, 450);
        positionCheck.normalizedRotation = AlignToTerrain(terrain, positionCheck.randomPosition, 20);
    }

    private Vector3 AlignToTerrain(Terrain terrain, float minX, float maxX, float minY, float maxY)
    {
        Vector3 randomizedPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minY, maxY));

        RaycastHit hit;
        Physics.Raycast(new Vector3(randomizedPosition.x, 500, randomizedPosition.z), -Vector3.up, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"));

        randomizedPosition.y = hit.point.y;
        return randomizedPosition;
    }

    private Quaternion AlignToTerrain(Terrain terrain, Vector3 randomPosition, float maxAngle, bool checkAngle = true)
    {
        Vector3 normalizedRotationAtPoint = terrain.terrainData.GetInterpolatedNormal(randomPosition.x / terrain.terrainData.size.x, randomPosition.z / terrain.terrainData.size.z);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalizedRotationAtPoint);

        if (checkAngle && Quaternion.Angle(rotation, Quaternion.identity) > maxAngle)
            rotation = Quaternion.AngleAxis(maxAngle, Vector3.up);

        return rotation * Quaternion.Euler(Vector3.up * Random.Range(-180f, 180f));
    }
}
