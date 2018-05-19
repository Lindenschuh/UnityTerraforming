using Invector.vCamera;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject DestinationPrefab;
    public GameObject SpawnerPrefab;

    [HideInInspector]
    public GameObject MainDestination;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    private void Start()
    {
        Transform spawnPoint;

        spawnPoint = GameObject.Find("SpawnPoint_Destination").transform;
        GameObject destinationGO = Instantiate(DestinationPrefab, spawnPoint);
        MainDestination = destinationGO.gameObject;

        spawnPoint = GameObject.Find("SpawnPoint_Spawner").transform;
        GameObject spawner = Instantiate(SpawnerPrefab, spawnPoint);

        spawnPoint = GameObject.Find("SpawnPoint_" + PhotonNetwork.player.NickName).transform;
        PhotonNetwork.Instantiate(PhotonNetwork.player.NickName, spawnPoint.position, spawnPoint.rotation, 0);

        if (PhotonNetwork.player.NickName == "Priest")
        {
            Debug.LogWarning(PhotonNetwork.player.NickName);
            GameObject priest = GameObject.Find("Priest(Clone)");
            priest.GetComponentInChildren<vThirdPersonCamera>().enabled = true;
            priest.GetComponentInChildren<Camera>().enabled = true;
            priest.GetComponent<BuildMode>().enabled = true;
            priest.GetComponent<vThirdPersonInput>().enabled = true;
            priest.GetComponent<vThirdPersonController>().enabled = true;
        }
        else
        {
            GameObject god = GameObject.Find("God(Clone)");
            god.GetComponentInChildren<Camera>().enabled = true;
            god.GetComponentInChildren<SimpleCameraMovement>().enabled = true;
            Camera mainCamera = GameObject.Find("God(Clone)").GetComponentInChildren<Camera>();
            Debug.LogWarning(PhotonNetwork.player.NickName);
            GameObject terrain = GameObject.Find("Terrain");
            terrain.GetComponent<TerraManipulation>().MainCamera = mainCamera;
        }
    }
}