using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // DEBUG START
        if (Input.GetMouseButtonDown(0))
        {
            const float maxDistance = 100f;
            Ray ray = GameObject.Find("DebugCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance).OrderBy(h => h.distance).ToArray();
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.collider.gameObject.name == "DebugStartButton")
                {
                    Debug.Log(hit.collider.gameObject.name);
                    OnClickedDebugButton();
                }
            }
        }
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

    public void OnClickedDebugButton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
        else if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // load game scene for VR
            SceneSpanningData.IsAssistant = false;
            PhotonNetwork.LoadLevel("GameScene"); //XDPaintDemo GameScene
        }
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
                SceneSpanningData.IsAssistant = _isAssistant;
                PhotonNetwork.LoadLevel("GameScene"); //XDPaintDemo GameScene
            }
        }
        else
        {
            JoinRoom();
        }
    }

    private void JoinRoom()
    {
        if (Application.isMobilePlatform)
        {
            // join a room that has an assistant
        }
        else
        {
            // join a room that needs an assistant
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickedQuitButton()
    {
        Application.Quit();
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
