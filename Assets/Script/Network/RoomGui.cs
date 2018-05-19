using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGui : Photon.PunBehaviour {

    public Text playerName;
    public Dropdown playerRole;
    public Dropdown playerTeam;
    public Toggle playerReady;
    public int playerID;

    public RoomGui(Text pPlayerName, Dropdown pPlayerRole, Dropdown pPlayerTeam, Toggle pPlayerReady, int pPlayerID)
    {
        playerName = Instantiate(pPlayerName);
        playerRole = Instantiate(pPlayerRole);
        playerTeam = Instantiate(pPlayerTeam);
        playerReady = Instantiate(pPlayerReady);
        playerID = pPlayerID;
    }

    public void attachToParent(GridLayoutGroup gridLayoutGroup)
    {
        playerName.transform.SetParent(gridLayoutGroup.transform);
        playerRole.transform.SetParent(gridLayoutGroup.transform);
        playerTeam.transform.SetParent(gridLayoutGroup.transform);
        playerReady.transform.SetParent(gridLayoutGroup.transform);
        playerName.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        playerRole.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        playerTeam.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        playerReady.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void destroyComponents()
    {
        playerName.transform.SetParent(null);
        Destroy(playerName);
        playerRole.transform.SetParent(null);
        Destroy(playerRole);
        playerTeam.transform.SetParent(null);
        Destroy(playerTeam);
        playerReady.transform.SetParent(null);
        Destroy(playerReady);
    }

}
