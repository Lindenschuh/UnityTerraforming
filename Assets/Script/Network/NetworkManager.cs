using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class NetworkManager : Photon.PunBehaviour
{

    #region Public Variables

    // pun standard settings
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public byte MaxPlayersPerRoom = 2;


    // Menu panels
    public GameObject mainMenuPanel;
    public GameObject userNameInputPanel;
    public GameObject lobbyPanel;
    public GameObject roomNameInputPanel;
    public GameObject roomPanel;


    // stuff to create tables
    public GridLayoutGroup roomGridLayout;
    public Button joinButton;
    public Text gameName;
    public Text playerCount;
    public Text gameType;

    public GridLayoutGroup playerGridLayout;
    public Text playerName;
    public Dropdown playerRole;
    public Dropdown playerTeam;
    public Toggle playerReady;

    public Dropdown maxPlayerCount;
    public Dropdown gameMode;


    #endregion


    #region Private Variables

    const string _gameVersion = "0.0.1";
    public Dictionary<RoomInfo, LobbyGui> lobby;
    public Dictionary<RoomInfo, List<RoomGui>> rooms;

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.logLevel = Loglevel;
        lobby = new Dictionary<RoomInfo, LobbyGui>();
        rooms = new Dictionary<RoomInfo, List<RoomGui>>();

    }

    void Start()
    {
        lobbyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    #endregion

    #region Photon.PunBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.LogWarning("Join Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnectedFromPhoton()
    {
        lobbyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        string playerName = PhotonNetwork.player.NickName;
        int playerID = PhotonNetwork.player.ID;
        photonView.RPC("RPCUpdateRoom", PhotonTargets.AllBufferedViaServer, playerName, playerID);
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }


    public override void OnPhotonMaxCccuReached()
    {
        base.OnPhotonMaxCccuReached();
    }

    public override void OnReceivedRoomListUpdate()
    {
        Debug.LogWarning("OnReceivedRoomListUpdate");
        CheckForNewLobbyEntries();
        CheckForOldLobbyEntries();
    }

    #endregion

    #region Helper Methods

    public void EnterName()
    {
        userNameInputPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.player.NickName = userNameInputPanel.transform.GetChild(1).GetChild(2).GetComponent<Text>().text;
    }

    public void EnterNameCancel()
    {
        userNameInputPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void Connect()
    {
        userNameInputPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void CreateNewRoom()
    {
        lobbyPanel.SetActive(false);
        roomNameInputPanel.SetActive(true);
    }

    public void CreateNewRoomCancel()
    {
        roomNameInputPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void ApplyRoom()
    {
        string roomName = roomNameInputPanel.transform.GetChild(1).GetChild(2).GetComponent<Text>().text;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        roomNameInputPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public static void JoinRoom(string roomName)
    {
        Debug.LogWarning(roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        try
        {
            if (rooms != null)
            {
                foreach (RoomGui roomGui in rooms[PhotonNetwork.room])
                {
                    roomGui.destroyComponents();
                }
                rooms.Clear();
            }

            photonView.RPC("RPCLeaveRoom", PhotonTargets.Others, PhotonNetwork.player.ID);
            PhotonNetwork.LeaveRoom();
        }
        catch (ArgumentNullException) { }
    }


    private void CheckForNewLobbyEntries()
    {
        foreach (RoomInfo element in PhotonNetwork.GetRoomList())
        {
            LobbyGui lobbyGui;
            if (!lobby.Keys.Contains(element))
            {
                lobbyGui = new LobbyGui(joinButton, gameName, playerCount, gameType);
                lobbyGui.joinButtonAddListener(element.Name);
                lobby.Add(element, lobbyGui);
            }

            foreach (RoomInfo roomInfo in lobby.Keys)
            {
                if (roomInfo.Equals(element))
                {
                    lobby.TryGetValue(roomInfo, out lobbyGui);
                    lobbyGui.updateValues(element);
                    lobbyGui.attachToParent(roomGridLayout);
                    break;
                }
            }
        }
    }

    private void CheckForOldLobbyEntries()
    {
        List<RoomInfo> oldRooms = new List<RoomInfo>();
        foreach (RoomInfo roomInfo in lobby.Keys)
        {
            if(!PhotonNetwork.GetRoomList().Contains(roomInfo))
            {
                oldRooms.Add(roomInfo);
            }
        }

        foreach (RoomInfo roomInfo in oldRooms)
        {
            lobby[roomInfo].destroyComponents();
            lobby.Remove(roomInfo);
        }
    }

    public void CheckForGameStart()
    {
        byte roleCounter = 0;
        List<RoomGui> roomGuis = rooms[PhotonNetwork.room];
        foreach (RoomGui roomGui in roomGuis)
        {
            if (roomGui.playerReady.isOn)
            {
                if (roomGui.playerRole.value == 1)
                    roleCounter++;
                else if (roomGui.playerRole.value == 2)
                    roleCounter--;
            }
            else
            {
                roleCounter = 1;
                break;
            }
        }

        if (roleCounter == 0)
        {
            string levelName = "Level_01";
            photonView.RPC("RPCStartGame", PhotonTargets.AllBufferedViaServer, levelName);
        }
    }

    private void ChangeRoomMaxPlayers(int pMaxPlayerCountValue)
    {
        if ((pMaxPlayerCountValue + 1) * 2 >= PhotonNetwork.room.PlayerCount)
        {
            PhotonNetwork.room.MaxPlayers = 2 + pMaxPlayerCountValue * 2;
            photonView.RPC("RPCChangeRoomMaxPlayers", PhotonTargets.OthersBuffered, pMaxPlayerCountValue);
        }
    }

    private void ChangePlayerRole(int pPlayerRoleValue, int pPlayerID)
    {
        photonView.RPC("RPCChangePlayerRole", PhotonTargets.OthersBuffered, pPlayerRoleValue, pPlayerID);
    }
    private void ChangePlayerTeam(int pPlayerTeamValue, int pPlayerID)
    {
        photonView.RPC("RPCChangePlayerTeam", PhotonTargets.OthersBuffered, pPlayerTeamValue, pPlayerID);
    }

    private void ChangePlayerReady(bool pPlayerReadyValue, int pPlayerID)
    {
        photonView.RPC("RPCChangePlayerReady", PhotonTargets.OthersBuffered, pPlayerReadyValue, pPlayerID);
    }



    #endregion

    #region PunRPC

    [PunRPC]
    private void RPCUpdateRoom(string pPlayerName, int pPlayerID)
    {
        if (!rooms.ContainsKey(PhotonNetwork.room))
        {
            rooms.Add(PhotonNetwork.room, new List<RoomGui>());
        }
        RoomGui roomGui = new RoomGui(playerName, playerRole, playerTeam, playerReady, pPlayerID);
        roomGui.playerName.text = pPlayerName;
        roomGui.playerRole.onValueChanged.AddListener(delegate { ChangePlayerRole(roomGui.playerRole.value, pPlayerID); });
        roomGui.playerTeam.onValueChanged.AddListener(delegate { ChangePlayerTeam(roomGui.playerTeam.value, pPlayerID); });
        roomGui.playerReady.onValueChanged.AddListener(delegate { ChangePlayerReady(roomGui.playerReady.isOn, pPlayerID); });
        maxPlayerCount.onValueChanged.AddListener(delegate { ChangeRoomMaxPlayers(maxPlayerCount.value); });
        List<RoomGui> roomGuiList = rooms[PhotonNetwork.room];
        roomGuiList.Add(roomGui);
        roomGui.attachToParent(playerGridLayout);
        if (PhotonNetwork.player.ID != pPlayerID)
        {
            roomGui.playerRole.interactable = false;
            roomGui.playerTeam.interactable = false;
            roomGui.playerReady.interactable = false;
        }
    }

    [PunRPC]
    private void RPCChangeRoomMaxPlayers(int pMaxPlayerCountValue)
    {
        maxPlayerCount.value = pMaxPlayerCountValue;
    }

    [PunRPC]
    private void RPCChangePlayerRole(int pRoleValue, int pPlayerID)
    {
        foreach (RoomGui roomGui in rooms[PhotonNetwork.room])
        {
            if (roomGui.playerID == pPlayerID)
            {
                roomGui.playerRole.value = pRoleValue;
            }
        }
    }

    [PunRPC]
    private void RPCChangePlayerTeam(int pTeamValue, int pPlayerID)
    {
        foreach (RoomGui roomGui in rooms[PhotonNetwork.room])
        {
            if (roomGui.playerID == pPlayerID)
            {
                roomGui.playerTeam.value = pTeamValue;
            }
        }
    }

    [PunRPC]
    private void RPCChangePlayerReady(bool pReadyValue, int pPlayerID)
    {
        foreach (RoomGui roomGui in rooms[PhotonNetwork.room])
        {
            if (roomGui.playerID == pPlayerID)
            {
                roomGui.playerReady.isOn = pReadyValue;
            }
        }
    }


    [PunRPC]
    private void RPCStartGame(string pLevelName)
    {
        int playerType = 0;
        string playerTypeText = "";
        foreach (RoomGui roomGui in rooms[PhotonNetwork.room])
        {
            if (roomGui.playerID == PhotonNetwork.player.ID)
            {
                playerType = roomGui.playerRole.value;
                break;
            }
        }
        switch (playerType)
        {
            case 1:
                playerTypeText = "God";
                break;
            case 2:
                playerTypeText = "Priest";
                break;
            default:
                playerTypeText = "Noob";
                break;
        }
        ExitGames.Client.Photon.Hashtable role = new ExitGames.Client.Photon.Hashtable();
        role.Add("role", playerTypeText);
        PhotonNetwork.player.SetCustomProperties(role);
        SceneManager.LoadScene("Level_01");
    }

    [PunRPC]
    private void RPCLeaveRoom(int pPlayerID)
    {
        RoomGui roomGuiTmp = null;
        foreach (RoomGui roomGui in rooms[PhotonNetwork.room])
        {
            if (roomGui.playerID == pPlayerID)
            {
                roomGuiTmp = roomGui;
                roomGui.destroyComponents();
                break;
            }
        }
        Debug.LogWarning("Roomgui:" + roomGuiTmp.playerID);
        rooms[PhotonNetwork.room].Remove(roomGuiTmp);
    }

    #endregion
}
