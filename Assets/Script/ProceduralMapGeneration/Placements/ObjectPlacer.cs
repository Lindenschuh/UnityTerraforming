using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPlacer : Photon.PunBehaviour
{        
    public GameObject[] prefabs;
    public int tries;
    public int prefabsCount;
    public Terrain terrain;
    public ObjectSettings[] objects;

    private float terrainWidth;
    private float terrainHeight;

    //void Start()
    //{ 
    //    if (PhotonNetwork.isMasterClient)
    //        StartCoroutine(SpawnObjects(prefabs, RandomizeOnTerrain, prefabsCount, tries));
    //}

    //private IEnumerator SpawnObjects(GameObject[] prefabsToSpawn, System.Action<PositionCheck> randomizeDelegate, int spawnCount, int maximumRetry = 50)
    //{
    //    foreach (GameObject prefab in prefabsToSpawn)
    //    {
    //        PositionCheck positionCheck = new PositionCheck();
    //        yield return StartCoroutine(SpawnObject(prefab, positionCheck, randomizeDelegate, Mathf.RoundToInt(spawnCount / prefabsToSpawn.Length), maximumRetry));
    //    }
    //}

    //IEnumerator SpawnObject(GameObject prefabToSpawn, PositionCheck positionCheck, System.Action<PositionCheck> randomizeDelegate, int spawnCount, int maximumRetry = 50, bool allowOverdraw = false)
    //{
    //    int currentObjectsCount = 0;
    //    int currentRetry = 0;

    //    if (prefabToSpawn.GetComponent<Collider>() == null)
    //        allowOverdraw = true;

    //    CheckForCollisions template = Instantiate(prefabToSpawn).AddComponent<CheckForCollisions>();


    //    while (currentObjectsCount < spawnCount && currentRetry < maximumRetry)
    //    {
    //        positionCheck.Reset();
    //        RandomizeOnTerrain(positionCheck);
    //        if (!allowOverdraw)
    //            yield return StartCoroutine(template.IsColliding(positionCheck));
    //        else
    //            positionCheck.PossitionAllowed();

    //        if (positionCheck.isColliding)
    //        {
    //            currentRetry++;
    //            continue;
    //        }
    //        PhotonNetwork.Instantiate(prefabToSpawn.name, positionCheck.randomPosition, positionCheck.normalizedRotation, 0);
    //        currentObjectsCount++;
    //    }
    //    Destroy(template.gameObject);
    //}

    //public void RandomizeOnTerrain(PositionCheck positionCheck)
    //{
    //    positionCheck.randomPosition = AlignToTerrain(terrain, 50, 450, 50, 450);
    //    positionCheck.normalizedRotation = AlignToTerrain(terrain, positionCheck.randomPosition, 20);
    //}

    //private Vector3 AlignToTerrain(Terrain terrain, float minX, float maxX, float minY, float maxY)
    //{
    //    Vector3 randomizedPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minY, maxY));

    //    RaycastHit hit;
    //    Physics.Raycast(new Vector3(randomizedPosition.x, 500, randomizedPosition.z), -Vector3.up, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"));

    //    randomizedPosition.y = hit.point.y;
    //    return randomizedPosition;
    //}

    //private Quaternion AlignToTerrain(Terrain terrain, Vector3 randomPosition, float maxAngle, bool checkAngle = true)
    //{
    //    Vector3 normalizedRotationAtPoint = terrain.terrainData.GetInterpolatedNormal(randomPosition.x / terrain.terrainData.size.x, randomPosition.z / terrain.terrainData.size.z);
    //    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalizedRotationAtPoint);

    //    if (checkAngle && Quaternion.Angle(rotation, Quaternion.identity) > maxAngle)
    //        rotation = Quaternion.AngleAxis(maxAngle, Vector3.up);

    //    return rotation * Quaternion.Euler(Vector3.up * Random.Range(-180f, 180f));
    //}

    void Start()
    {
        // get size of terrain
        terrainWidth = terrain.terrainData.heightmapWidth / 2;
        terrainHeight = terrain.terrainData.heightmapHeight / 2;

        if (PhotonNetwork.isMasterClient)
            StartCoroutine(SpawnObjects(objects, RandomizeOnTerrain, prefabsCount, tries));
    }

    private IEnumerator SpawnObjects(ObjectSettings[] prefabsToSpawn, System.Action<PositionCheck, float[]> randomizeDelegate, int spawnCount, int maximumRetry = 50)
    {
        foreach (ObjectSettings prefab in prefabsToSpawn)
        {
            PositionCheck positionCheck = new PositionCheck();
            yield return StartCoroutine(SpawnObject(prefab, positionCheck, randomizeDelegate, maximumRetry));
        }
    }

    IEnumerator SpawnObject(ObjectSettings prefabToSpawn, PositionCheck positionCheck, System.Action<PositionCheck, float[]> randomizeDelegate, int maximumRetry = 50, bool allowOverdraw = false)
    {
        int currentObjectsCount = 0;
        int currentRetry = 0;
        float[][] positions = GetActualPosition(prefabToSpawn);

        if (prefabToSpawn.GetComponent<Collider>() == null)
            allowOverdraw = true;

        CheckForCollisions template = Instantiate(prefabToSpawn.preFab).AddComponent<CheckForCollisions>();


        while (currentObjectsCount < prefabToSpawn.amount * prefabToSpawn.differentLocations && currentRetry < maximumRetry)
        {
            int i = prefabToSpawn.amount;
            positionCheck.Reset();
            RandomizeOnTerrain(positionCheck, positions[i]);
            if (!allowOverdraw)
                yield return StartCoroutine(template.IsColliding(positionCheck));
            else
                positionCheck.PossitionAllowed();

            if (positionCheck.isColliding)
            {
                currentRetry++;
                continue;
            }
            PhotonNetwork.Instantiate(prefabToSpawn.preFab.name, positionCheck.randomPosition, positionCheck.normalizedRotation, 0);
            currentObjectsCount++;
            i--;
        }
        Destroy(template.gameObject);
    }

    private float[][] GetActualPosition(ObjectSettings prefabToSpawn)
    {
        // get Center of spawning circle with procentural values from settings
        float[] center = new float[] { terrainWidth - (prefabToSpawn.center * terrainWidth), terrainHeight - (prefabToSpawn.center * terrainHeight) };
        float outerWidth = center[0] - (prefabToSpawn.outerCircle * terrainWidth);
        float outerHeight = center[1] - (prefabToSpawn.outerCircle * terrainHeight);

        // check if outer circle is within terrain coords
        if (terrainWidth - center[0] < 0 || terrainWidth - outerWidth < 0)
        {
            center[0] = center[0] + outerWidth / 2;
            outerWidth = center[0] - (prefabToSpawn.outerCircle * terrainWidth);
        }

        if (terrainHeight - center[1] < 0 || terrainHeight - outerHeight < 0)
        {
            center[1] = center[1] + outerHeight / 2;
            outerHeight = center[1] - (prefabToSpawn.outerCircle * terrainHeight);
        }

        float innerWidth = center[0] - (prefabToSpawn.innerCircle * terrainWidth);
        float innerHeight = center[1] - (prefabToSpawn.innerCircle * terrainHeight);

        // areas of spawning
        float[][] positions = new float[prefabToSpawn.differentLocations][];
        for (int i = 0; i < prefabToSpawn.differentLocations; i++)
        {
            positions[i] = new float[4];
            int chances = 50;
            while (chances > 0)
            {
                float rndWidth = Random.Range(innerWidth, outerWidth);
                float rndHeight = Random.Range(innerHeight, outerHeight);
                if (i > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if ((positions[j][0] <= rndWidth && positions[j][1] >= rndWidth) && (positions[j][2] <= rndHeight && positions[j][3] >= rndHeight))
                        {
                            chances--;
                            break;
                        }
                    }
                }

                int size = prefabToSpawn.size / 2;
                positions[i][0] = rndWidth - size < 0 ? 0 : rndWidth - size;
                positions[i][1] = rndWidth + size > terrainWidth * 2 ? terrainWidth * 2 : rndWidth + size;
                positions[i][2] = rndHeight - size < 0 ? 0 : rndHeight - size;
                positions[i][3] = rndHeight + size > terrainHeight * 2 ? terrainHeight * 2 : rndHeight + size;
                break;
            }
        }
        return positions;
    }

    public void RandomizeOnTerrain(PositionCheck positionCheck, float[] borders)
    {
        positionCheck.randomPosition = AlignToTerrain(terrain, borders[0], borders[1], borders[2], borders[3]);
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
