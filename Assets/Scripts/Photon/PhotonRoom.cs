using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks,IInRoomCallbacks
{
    public static PhotonRoom room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    private Player[] photonPlayers;
    private int[] powerDistribution = { 0, 0, 0 };
    public int playersInRoom;
    public int myNumberInRoom;

    public int playerInGame;
    public GameObject RegisterPanel;

    InputField initialsField;

    //Delayed start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart;

    private void Awake()
    {
        
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    

    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToStart = false;
        readyToCount = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime; //
        initialsField = RegisterPanel.GetComponent<InputField>();
    }

    void Update()
    {
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if(playersInRoom == 1)
            {
                RestartTime();
            }

            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart -= Time.deltaTime;
                }else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
              //  Debug.Log("Display time to start: " + timeToStart);
                if (timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are in a room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = initialsField.text; ///ffffff
        if(MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log("Players in room: " + playersInRoom + " of " + MultiplayerSettings.multiplayerSettings.maxPlayer); //
            Debug.Log(PhotonNetwork.NickName);
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }

            if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayer)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined");
        photonPlayers = PhotonNetwork.PlayerList; //
        playersInRoom++; //
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayer)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    private void StartGame()
    {
        isGameLoaded = true;
        if (PhotonNetwork.IsMasterClient)
        {
            int numberOfPowerMaster = ((UnityEngine.Random.Range(1, 101)) % 2) + 1;
            int numberOfPowerSlave = 3 - numberOfPowerMaster;
            Debug.Log("Number Power for p1" + numberOfPowerMaster);
            Debug.Log("Number Power for p2" + numberOfPowerSlave);
            for (int i = 0; i < numberOfPowerSlave; ++i)
            {
                int randomPower = UnityEngine.Random.Range(1, 101) % 3;
                powerDistribution[randomPower] = 1;
            }

            
        }
        


        if (!PhotonNetwork.IsMasterClient)
            return;
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }

    private void RestartTime()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;

            if (MultiplayerSettings.multiplayerSettings.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }



    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playerInGame++;
        if (playerInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_SetPowerPrefs", RpcTarget.All, powerDistribution);
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
            
        }
    }

    [PunRPC]
    private void RPC_SetPowerPrefs(int [] powerDist)
    {
        PlayerPrefs.SetInt("Water", powerDist[0]);
        PlayerPrefs.SetInt("Fire", powerDist[1]);
        PlayerPrefs.SetInt("Earth", powerDist[2]);
        Debug.Log("Powa: " + powerDist[0]);
        Debug.Log("Powa: " + powerDist[1]);
        Debug.Log("Powa: " + powerDist[2]);
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), new Vector3(0.0f,0.0f,0.0f), Quaternion.identity, 0);
    }
}
