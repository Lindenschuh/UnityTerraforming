using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTerraforming.GameAi;

public class ObjectPlacer : Photon.PunBehaviour
{        
    public int tries;
    public Terrain terrain;
    public ObjectSettings[] objects;

    private float terrainWidth;
    private float terrainHeight;
    private string mainDestination = "Main Destination(Clone)";

    private struct AllowedMapData
    {
        public int width;
        public int height;
        public float heightValue;

        public AllowedMapData(int width, int height, float heightValue)
        {
            this.width = width;
            this.height = height;
            this.heightValue = heightValue;
        }
    }


    public void StartSpawn()
    {
        // get size of terrain
        terrainWidth = terrain.terrainData.heightmapWidth / 2;
        terrainHeight = terrain.terrainData.heightmapHeight / 2;

        if (PhotonNetwork.isMasterClient)
            StartCoroutine(SpawnObjects(objects, RandomizeOnTerrain, objects.Length, tries));
    }

    private IEnumerator SpawnObjects(ObjectSettings[] prefabsToSpawn, System.Action<PositionCheck, float[]> randomizeDelegate, int spawnCount, int tries = 100)
    {
        foreach (ObjectSettings prefab in prefabsToSpawn)
        {
            PositionCheck positionCheck = new PositionCheck();
            float[][] positions = GetActualPosition(prefab, tries);
            int spawnPositionAmount = 0;
            foreach (float[] heightValue in positions)
            {
                if (heightValue[0] == 0 && heightValue[1] == 0 && heightValue[2] == 0 && heightValue[3] == 0)
                    break;
                spawnPositionAmount++;
            }
            for (int i = 0; i < spawnPositionAmount; i++)
                yield return StartCoroutine(SpawnObject(prefab, positionCheck, randomizeDelegate, positions[i], tries));
        }
    }

    IEnumerator SpawnObject(ObjectSettings prefabToSpawn, PositionCheck positionCheck, System.Action<PositionCheck, float[]> randomizeDelegate, float[] position, int tries = 50, bool allowOverdraw = false)
    {
        int currentObjectsCount = 0;
        int currentRetry = 0;

        if (prefabToSpawn.GetComponent<Collider>() == null)
            allowOverdraw = true;

        CheckForCollisions template = Instantiate(prefabToSpawn.preFab).AddComponent<CheckForCollisions>();


        while (currentObjectsCount < prefabToSpawn.amount && currentRetry < tries)
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
            positionCheck.randomPosition = new Vector3(positionCheck.randomPosition.x, positionCheck.randomPosition.y - 0.5f, positionCheck.randomPosition.z);
            if (prefabToSpawn.preFab.gameObject.layer == 11)
                PhotonNetwork.Instantiate(prefabToSpawn.preFab.name, positionCheck.randomPosition, positionCheck.normalizedRotation, 0);
            else
                photonView.RPC("RPCSpawnObject", PhotonTargets.All, prefabToSpawn.preFab.name, positionCheck.randomPosition, positionCheck.normalizedRotation);
            
            currentObjectsCount++;
        }
        Destroy(template.gameObject);
    }

    private float[][] GetActualPosition(ObjectSettings prefabToSpawn, int tries)
    {
        List<AllowedMapData> allowedPlaces = new List<AllowedMapData>();
        float actualHeight;
        for (int i = 0; i < terrainWidth * 2; i++)
        {
            for (int j = 0; j < terrainHeight * 2; j++)
            {
                actualHeight = terrain.terrainData.GetHeight(i, j);
                if (actualHeight <= prefabToSpawn.maxHeight || prefabToSpawn.maxHeight == 0)
                    allowedPlaces.Add(new AllowedMapData(i, j, actualHeight));
            }
        }

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
        float outerWidth = prefabToSpawn.outerCircle * terrainWidth;
        float outerHeight = prefabToSpawn.outerCircle * terrainHeight;

        // check if outer circle is within terrain coords
        // check width
        if (center[0] - outerWidth < 0)
        {
            outerWidth = center[0] - 1;
        }
        else if(center[0] + outerWidth > terrainWidth * 2)
        {
            outerWidth = ((terrainWidth * 2) - center[0]) - 1;
        }

        //check height
        if (center[1] - outerHeight < 0)
        {
            outerHeight = center[1] - 1;
        }
        else if (center[1] + outerHeight > terrainHeight * 2)
        {
            outerHeight = ((terrainHeight * 2) - center[1]) - 1;
        }

        float innerWidth = prefabToSpawn.innerCircle * terrainWidth;
        float innerHeight = prefabToSpawn.innerCircle * terrainHeight;

        // areas of spawning
        float[][] positions = new float[prefabToSpawn.differentLocations][];
        for (int i = 0; i < prefabToSpawn.differentLocations; i++)
        {
            positions[i] = new float[4];
            float rndWidth = 0;
            float rndHeight = 0;
            while (tries > 0)
            {
                if (prefabToSpawn.preFab.gameObject.layer == 11 ^ prefabToSpawn.preFab.GetComponent<GuardianSpawner>())
                    tries = 2;
                bool spawnOk = true;

                AllowedMapData placeToCheck = allowedPlaces[Random.Range(0, allowedPlaces.Count)];
                if (((placeToCheck.width < center[0] + outerWidth && placeToCheck.width > center[0] + innerWidth) ^ (placeToCheck.width > center[0] - outerWidth && placeToCheck.width < center[0] - innerWidth)) && 
                    ((placeToCheck.height < center[1] + outerHeight && placeToCheck.height > center[1] + innerHeight) ^ (placeToCheck.height > center[1] - outerHeight && placeToCheck.height < center[1] - innerHeight)))
                { 
                    rndWidth = placeToCheck.width;
                    rndHeight = placeToCheck.height;
                }
                else
                {
                    tries--;
                    continue;
                }

                foreach (AllowedMapData allowedMapData in allowedPlaces)
                {
                    if (allowedMapData.width == (int)rndWidth && allowedMapData.height == (int)rndHeight)
                    {
                        spawnOk = true;
                        break;
                    }
                    else
                    {
                        spawnOk = false;
                    }

                }

                if (i > 0 && spawnOk)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if ((positions[j][0] <= rndWidth + prefabToSpawn.distanceBetweenLocations && positions[j][1] >= rndWidth - prefabToSpawn.distanceBetweenLocations) && (positions[j][2] <= rndHeight + prefabToSpawn.distanceBetweenLocations && positions[j][3] >= rndHeight - prefabToSpawn.distanceBetweenLocations))
                        {
                            tries--;
                            spawnOk = false;
                            break;
                        }
                    }
                }

                if (spawnOk && tries > 0)
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

    [PunRPC]
    private void RPCSpawnObject(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load(prefabName) as GameObject;
        GameObject spawnedObject = Instantiate(prefab, position, rotation);
        if (prefab.transform.GetComponent<GuardianSpawner>())
        {
            spawnedObject.transform.GetComponent<GuardianSpawner>().MainDestination = GameObject.Find(mainDestination).transform;
            GameManager gameManager = GameManager.instance;
            if (gameManager != null)
                gameManager.Spawners.Add(spawnedObject);
        }
    }
}
