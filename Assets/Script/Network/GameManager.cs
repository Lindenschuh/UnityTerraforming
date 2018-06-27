using Invector.vCamera;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vCharacterController.vActions;
using UnityTerraforming.GameAi;
using Invector;
using Invector.vMelee;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GodStateManager godStateManager;
    public List<GameObject> Spawners = new List<GameObject>();

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
            GameObject.Find("GodUI").SetActive(false);
            GameObject priest = GameObject.FindGameObjectWithTag(PlayerTag);
            GameObject.Find("UI").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("GodController").SetActive(false);
            priest.GetComponent<vThirdPersonController>().enabled = true;
            priest.transform.GetChild(5).gameObject.SetActive(true);
            priest.GetComponent<vShooterMeleeInput>().enabled = true;
            priest.GetComponent<vShooterManager>().enabled = true;
            priest.GetComponent<vAmmoManager>().enabled = true;
            priest.GetComponent<vHeadTrack>().enabled = true;
            priest.GetComponent<vRagdoll>().enabled = true;
            priest.GetComponent<vFootStep>().enabled = true;
            priest.GetComponent<vWeaponHolderManager>().enabled = true;
            priest.GetComponent<vCollectShooterMeleeControl>().enabled = true;
            priest.GetComponent<vLockOnShooter>().enabled = true;
            priest.GetComponent<vGenericAction>().enabled = true;
            priest.GetComponent<vMeleeManager>().enabled = true;
            priest.GetComponent<BuildMode>().enabled = true;
            priest.GetComponent<ResourceControl>().enabled = true;           
            priest.GetComponent<UIControl>().enabled = true;
            priest.transform.Find("vThirdPersonCamera").gameObject.SetActive(true);
            priest.GetComponentInChildren<Health>().enabled = true;
            priest.GetComponent<BuildTrap>().enabled = true;

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

    private void Update()
    {
        int i = 0;
        i = i + 1;
    }
}