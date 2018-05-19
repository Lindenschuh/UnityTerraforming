using Invector.vCamera;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vCharacterController.vActions;

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

        spawnPoint = GameObject.Find("SpawnPoint_" + PhotonNetwork.player.CustomProperties["role"].ToString()).transform;
        PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties["role"].ToString(), spawnPoint.position, spawnPoint.rotation, 0);

        if (PhotonNetwork.player.CustomProperties["role"].ToString() == "Priest")
        {
            Debug.LogWarning(PhotonNetwork.player.NickName);
            GameObject priest = GameObject.Find("Priest(Clone)");
            priest.GetComponent<vThirdPersonController>().enabled = true;
            priest.transform.GetChild(0).gameObject.SetActive(true);
            priest.GetComponent<vShooterMeleeInput>().enabled = true;
            priest.GetComponent<vShooterManager>().enabled = true;
            priest.GetComponent<vAmmoManager>().enabled = true;
            priest.GetComponent<vHeadTrack>().enabled = true;
            priest.GetComponent<vGenericAction>().enabled = true;
            priest.GetComponent<BuildMode>().enabled = true;
            priest.GetComponentInChildren<vThirdPersonCamera>().enabled = true;
            priest.GetComponentInChildren<Camera>().enabled = true;
        }
        else
        {
            GameObject god = GameObject.Find("God(Clone)");
            god.GetComponentInChildren<Camera>().enabled = true;
            god.GetComponentInChildren<SimpleCameraMovement>().enabled = true;
            Camera mainCamera = GameObject.Find("God(Clone)").GetComponentInChildren<Camera>();
            GameObject terrain = GameObject.Find("Terrain");
            terrain.GetComponent<TerraManipulation>().MainCamera = mainCamera;
        }
    }
}