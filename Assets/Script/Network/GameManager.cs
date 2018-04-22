using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.networkingPeer.QuickResendAttempts = 4;
        PhotonNetwork.networkingPeer.SentCountAllowance = 7;
        Debug.LogWarning("Spielername ist: " + PhotonNetwork.player.NickName);
        Transform spawnPoint = GameObject.Find("SpawnPoint_" + PhotonNetwork.player.NickName).transform;
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

    // Update is called once per frame
    void Update () {
		
	}
}
