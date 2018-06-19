using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPlacer : Photon.PunBehaviour
{        
    public int tries;
    public int prefabsCount;
    public Terrain terrain;
    public ObjectSettings[] objects;

    private float terrainWidth;
    private float terrainHeight;


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
            float[][] positions = GetActualPosition(prefab);
            for (int i = 0; i < positions.GetLength(0); i++)
                yield return StartCoroutine(SpawnObject(prefab, positionCheck, randomizeDelegate, positions[i], maximumRetry));
        }
    }

    IEnumerator SpawnObject(ObjectSettings prefabToSpawn, PositionCheck positionCheck, System.Action<PositionCheck, float[]> randomizeDelegate, float[] position, int maximumRetry = 50, bool allowOverdraw = false)
    {
        int currentObjectsCount = 0;
        int currentRetry = 0;

        if (prefabToSpawn.GetComponent<Collider>() == null)
            allowOverdraw = true;

        CheckForCollisions template = Instantiate(prefabToSpawn.preFab).AddComponent<CheckForCollisions>();


        while (currentObjectsCount < prefabToSpawn.amount && currentRetry < maximumRetry)
        {
            positionCheck.Reset();
            RandomizeOnTerrain(positionCheck, position);
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
        }
        Destroy(template.gameObject);
    }

    private float[][] GetActualPosition(ObjectSettings prefabToSpawn)
    {
        // check if spawnpoint is within map
        if (prefabToSpawn.point.x > terrainWidth)
            prefabToSpawn.point.x = terrainWidth;
        else if (prefabToSpawn.point.x < 0)
            prefabToSpawn.point.x = 0;

        if (prefabToSpawn.point.y > terrainHeight)
            prefabToSpawn.point.y = terrainHeight;
        else if (prefabToSpawn.point.y < 0)
            prefabToSpawn.point.y = 0;

        // get Center of spawning circle with procentural values from settings
        float[] center = new float[] {prefabToSpawn.point.x, prefabToSpawn.point.y };
        float outerWidth = center[0] + (prefabToSpawn.outerCircle * terrainWidth);
        float outerHeight = center[1] + (prefabToSpawn.outerCircle * terrainHeight);

        // check if outer circle is within terrain coords
        if (terrainWidth - center[0] < 0) // || outerWidth - terrainWidth < 0)
        {
            center[0] = center[0] + outerWidth;
            outerWidth = center[0] + (prefabToSpawn.outerCircle * terrainWidth);
        }

        if (terrainHeight - center[1] < 0) // || outerHeight - terrainHeight < 0)
        {
            center[1] = center[1] + outerHeight;
            outerHeight = center[1] + (prefabToSpawn.outerCircle * terrainHeight);
        }

        float innerWidth = center[0] + (prefabToSpawn.innerCircle * terrainWidth);
        float innerHeight = center[1] + (prefabToSpawn.innerCircle * terrainHeight);
        // areas of spawning

        float[][] positions = new float[prefabToSpawn.differentLocations][];
        for (int i = 0; i < prefabToSpawn.differentLocations; i++)
        {
            positions[i] = new float[4];
            float rndWidth = 0;
            float rndHeight = 0;
            int chances = 50;
            while (chances > 0)
            {
                bool spawnOk = true;

                int check = Random.Range(0, 4);

                if (check <= 1)
                {
                    rndWidth = Random.Range(center[0] - (outerWidth - center[0]), outerWidth);
                    rndHeight = Random.Range(innerHeight, outerHeight);
                }
                else if (check <= 2)
                {
                    rndWidth = Random.Range(innerWidth, outerWidth);
                    rndHeight = Random.Range(center[1] - (outerHeight - center[1]), outerHeight);
                }
                else
                {
                    rndWidth = Random.Range(innerWidth, outerWidth);
                    rndHeight = Random.Range(innerHeight, outerHeight);
                }

                if (Random.Range(0, 100) <= 50)
                    rndWidth = center[0] - (rndWidth - center[0]);
                if (Random.Range(0, 100) <= 50)
                    rndHeight = center[1] - (rndHeight - center[1]);

                if (i > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if ((positions[j][0] <= rndWidth + prefabToSpawn.distanceBetweenLocations && positions[j][1] >= rndWidth - prefabToSpawn.distanceBetweenLocations) && (positions[j][2] <= rndHeight + prefabToSpawn.distanceBetweenLocations && positions[j][3] >= rndHeight - prefabToSpawn.distanceBetweenLocations))
                        {
                            chances--;
                            spawnOk = false;
                            break;
                        }
                    }
                }
                if (spawnOk && chances > 0)
                {
                    int size = prefabToSpawn.size / 2;
                    positions[i][0] = rndWidth - size < 0 ? 0 : rndWidth - size;
                    positions[i][1] = rndWidth + size > terrainWidth * 2 ? terrainWidth * 2: rndWidth + size;
                    positions[i][2] = rndHeight - size < 0 ? 0 : rndHeight - size;
                    positions[i][3] = rndHeight + size > terrainHeight * 2 ? terrainHeight * 2 : rndHeight + size;
                    break;
                }
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
