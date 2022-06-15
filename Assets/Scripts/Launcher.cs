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
    private Toggle assistantToggle;

    [SerializeField]
    private TextPanel infoTextPanel;

    [SerializeField]
    private LightsPanel lightsPanel;

    [SerializeField]
    private string gameVersion;

    [SerializeField]
    private byte maxPlayersPerRoom;

    [SerializeField]
    private Animation intro;

    [SerializeField]
    private GameObject menu;


    private bool _isAssistant;
    private bool _gameHasAssistant;
    private ButtonVisuals _buttonVisuals;

    private PhotonView _photonView;


    private void Awake()
    {
        // load a new scene on all clients
        PhotonNetwork.AutomaticallySyncScene = true;
        _buttonVisuals = menu.GetComponentInChildren<ButtonVisuals>();
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        StartCoroutine(Intro());
    }


    void Update()
    {
        
    }

    public IEnumerator Intro()
    {
        intro.Play();
        while (intro.isPlaying)
        {
            yield return new WaitForSeconds(1);
        }
        menu.SetActive(true);
        yield return new WaitForSeconds(1);
        ConnectToPhoton();
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        infoTextPanel.WriteLine("Connected to Photon master. We are online!");
        lightsPanel.SetGreen(GUIConstants.IndicatorLight.PHOTON);
    }

    public void OnClickedConnectButton()
    {
    }

    public void OnClickedPlayButton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            infoTextPanel.WriteLine("We are connecting to the network. Please wait.");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            // start game
            if (!PhotonNetwork.IsMasterClient)
                infoTextPanel.WriteLine("Sorry, only your boss is allowed to start the game.");
            else if (!_gameHasAssistant)
                infoTextPanel.WriteLine("Chief, you cannot start without an assistant!");
            else
            {
                StaticClass.IsAssistant = _isAssistant;
                PhotonNetwork.LoadLevel("XDPaintDemo"); //XDPaintDemo GameScene
            }
        }
        else
        {
            // join room
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void OnAssistantToggleChanged()
    {
        _isAssistant = assistantToggle.isOn;
        Debug.Log("is on: " + assistantToggle.isOn);
        if (_isAssistant)
            _gameHasAssistant = true;
        _photonView.RPC("OnRemoteAssistantChanged", RpcTarget.Others, _isAssistant);
    }

    [PunRPC]
    public void OnRemoteAssistantChanged(bool other)
    {
        Debug.Log("Remote is assistant");
        _isAssistant = !other;
        assistantToggle.isOn = !other;
        _gameHasAssistant = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        infoTextPanel.WriteLine("Create room...");
    }

    public override void OnJoinedRoom()
    {
        infoTextPanel.WriteLine("Joined room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        assistantToggle.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not called if I am joining myself
        infoTextPanel.WriteLine("Player joined room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        infoTextPanel.WriteLine("Player left room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
