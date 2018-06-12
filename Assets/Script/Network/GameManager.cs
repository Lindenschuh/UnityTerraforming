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

    public GodStateManager godStateManager;

    private readonly string God = "God(Clone)";
    private readonly string Priest = "Priest";
    private readonly string Terrain = "Terrain";
    private readonly string PropertyRole = "role";
    private readonly string PlayerTag = "Player";

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

        spawnPoint = GameObject.Find($"SpawnPoint_{ PhotonNetwork.player.CustomProperties[PropertyRole].ToString() }").transform;
        PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties[PropertyRole].ToString(), spawnPoint.position, spawnPoint.rotation, 0);

        if (PhotonNetwork.player.CustomProperties[PropertyRole].ToString() == Priest)
        {

            Debug.LogWarning(PhotonNetwork.player.NickName);
            GameObject.Find("GodUI").SetActive(false);
            GameObject.Find("God").SetActive(false);
            GameObject priest = GameObject.FindGameObjectWithTag(PlayerTag);
            GameObject.Find("UI").transform.GetChild(0).gameObject.SetActive(true);
            priest.GetComponent<vThirdPersonController>().enabled = true;
            priest.transform.GetChild(0).gameObject.SetActive(true);
            priest.GetComponent<vShooterMeleeInput>().enabled = true;
            priest.GetComponent<vShooterManager>().enabled = true;
            priest.GetComponent<vAmmoManager>().enabled = true;
            priest.GetComponent<vHeadTrack>().enabled = true;
            priest.GetComponent<vGenericAction>().enabled = true;
            priest.GetComponent<BuildMode>().enabled = true;
            priest.GetComponent<vItemManager>().enabled = true;
            priest.GetComponent<ResourceControl>().enabled = true;           
            priest.GetComponent<UIControl>().enabled = true;
            priest.transform.Find("vThirdPersonCamera").gameObject.SetActive(true);
                  
            //priest.GetComponentInChildren<Camera>().enabled = true;
            //priest.GetComponentInChildren<vThirdPersonCamera>().enabled = true;
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
}