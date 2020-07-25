using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;


    public GameObject battleButton;
    public GameObject cancelButton;
    public GameObject RegisterPanel;
   
    InputField initialsField;


    private void Awake()
    {
        lobby = this;

    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        initialsField = RegisterPanel.GetComponent<InputField>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon Master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.SetActive(true);
        RegisterPanel.SetActive(true);
    }

    

    public void OnBattleButtonClick()
    {
        battleButton.SetActive(false);
        RegisterPanel.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Se registro el jugador: " + initialsField.text);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random game but failed");
        CreateRoom();
    }

    private void CreateRoom() {
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayer };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a room");
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("We are in a room");
    }

    public void OnCancelButtonClick()
    {
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        RegisterPanel.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
