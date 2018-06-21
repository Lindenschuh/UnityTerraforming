using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Invector.vCharacterController;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vCharacterController.vActions;

public class DebugNetworkManager : Photon.PunBehaviour
{

    #region Public Variables

    // pun standard settings
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public byte MaxPlayersPerRoom = 1;
    public Transform Player;
    public GodStateManager godStateManager;
    #endregion


    #region Private Variables

    const string _gameVersion = "0.0.1";
    private readonly string God = "God(Clone)";
    private readonly string Priest = "Priest";
    private readonly string Terrain = "Terrain";
    private readonly string PropertyRole = "role";
    private readonly string PlayerTag = "Player";

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.logLevel = Loglevel;
        PhotonNetwork.offlineMode = true;
        //PhotonNetwork.ConnectUsingSettings(_gameVersion);

    }

    private void Start()
    {
        Transform spawnPoint;

        spawnPoint = GameObject.Find($"SpawnPoint_{ Player.name }").transform;
        PhotonNetwork.Instantiate(Player.name, spawnPoint.position, spawnPoint.rotation, 0);

        if (Player.name == Priest)
        {
            GameObject.Find("GodUI").SetActive(false);
            GameObject priest = GameObject.FindGameObjectWithTag(PlayerTag);
            GameObject.Find("UI").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("GodController").SetActive(false);
            priest.GetComponent<vThirdPersonController>().enabled = true;
            priest.transform.GetChild(0).gameObject.SetActive(true);
            priest.GetComponent<vShooterMeleeInput>().enabled = true;
            priest.GetComponent<vShooterManager>().enabled = true;
            priest.GetComponent<vAmmoManager>().enabled = true;
            priest.GetComponent<vHeadTrack>().enabled = true;
            priest.GetComponent<vGenericAction>().enabled = true;
            priest.GetComponent<BuildMode>().enabled = true;
            //priest.GetComponent<vItemManager>().enabled = true;
            priest.GetComponent<ResourceControl>().enabled = true;
            priest.GetComponent<UIControl>().enabled = true;
            priest.transform.Find("vThirdPersonCamera").gameObject.SetActive(true);
            priest.GetComponentInChildren<Health>().enabled = true;
        }
        else
        {
            GameObject god = GameObject.Find(God);
            god.GetComponentInChildren<Camera>().enabled = true;
            god.GetComponentInChildren<SimpleCameraMovement>().enabled = true;
            Camera mainCamera = GameObject.Find(God).GetComponentInChildren<Camera>();
            mainCamera.GetComponent<SimpleCameraMovement>().GodState = godStateManager;
            GameObject terrain = GameObject.Find(Terrain);
            terrain.GetComponent<TerraManipulation>().MainCamera = mainCamera;
            GameObject.Find("UI").SetActive(false);
        }
    }

    #endregion

    #region Photon.PunBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    #endregion
}
