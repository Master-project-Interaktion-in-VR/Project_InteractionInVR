using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Button connectButton;

    [SerializeField]
    private Button playButton;

    [SerializeField]
    private TMPro.TextMeshProUGUI infoText;

    [SerializeField]
    private string gameVersion;

    [SerializeField]
    private byte maxPlayersPerRoom;


    private void Awake()
    {
        // load a new scene on all clients
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.interactable = false;
    }

    void Start()
    {

    }


    void Update()
    {
        
    }

    public void OnClickedConnectButton()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void OnClickedPlayButton()
    {
        if (PhotonNetwork.InRoom)
        {
            // start game
            if (!PhotonNetwork.IsMasterClient)
                infoText.text = infoText.text + "\n" + "Master client starts the game";
            else
                PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            // join room
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        infoText.text = infoText.text + "\n" + "Connected to Photon master";
        connectButton.interactable = false;
        playButton.interactable = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        infoText.text = infoText.text + "\n" + "Create room...";
    }

    public override void OnJoinedRoom()
    {
        infoText.text = infoText.text + "\n" + "Joined room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not called if I am joining myself

        infoText.text = infoText.text + "\n" + "Player joined room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        infoText.text = infoText.text + "\n" + "Player left room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount;
    }
}
