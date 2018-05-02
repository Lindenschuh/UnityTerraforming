using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGui : MonoBehaviour {

    public Button joinButton;
    public Text gameName;
    public Text playerCount;
    public Text gameType;

    private const string joinButtonName = "joinButton";

    public LobbyGui(Button pJoinButton, Text pGameName, Text pPlayerCount, Text pGameType)
    {
        joinButton = Instantiate(pJoinButton);
        gameName = Instantiate(pGameName);
        playerCount = Instantiate(pPlayerCount);
        gameType = Instantiate(pGameType);
    }

    public void attachToParent(GridLayoutGroup gridLayoutGroup)
    {
        joinButton.transform.SetParent(gridLayoutGroup.transform);
        gameName.transform.SetParent(gridLayoutGroup.transform);
        playerCount.transform.SetParent(gridLayoutGroup.transform);
        gameType.transform.SetParent(gridLayoutGroup.transform);
        joinButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        gameName.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        playerCount.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        gameType.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void updateValues(RoomInfo roomInfo)
    {
        joinButton.name = joinButtonName + roomInfo.Name;
        gameName.text = roomInfo.Name;
        playerCount.text = string.Format("{0}/{1}", roomInfo.PlayerCount, roomInfo.MaxPlayers);
        gameType.text = "Defense";
    }

    public void joinButtonAddListener(string pGameName)
    {
        joinButton.onClick.AddListener(delegate { NetworkManager.JoinRoom(pGameName); });
    }

    public void destroyComponents()
    {
        joinButton.transform.SetParent(null);
        Destroy(joinButton);
        gameName.transform.SetParent(null);
        Destroy(gameName);
        playerCount.transform.SetParent(null);
        Destroy(playerCount);
        gameType.transform.SetParent(null);
        Destroy(gameType);
    }
}
