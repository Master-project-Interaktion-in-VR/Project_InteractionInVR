using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    private static bool DEBUG = false;
    // DEBUG: menu scene with mouse (PC view), skip intro, enable debug scene load

    [SerializeField]
    private string gameVersion;

    [SerializeField]
    private Animation intro;

    [SerializeField]
    private TextPanel infoTextPanel;

    [SerializeField]
    private LightsPanel lightsPanel;

    [SerializeField]
    private LightsPanel playerReadyPanel;


    [Header("Menus")]

    [SerializeField]
    private GameObject menu;

    [SerializeField]
    private GameObject mainMenuContainer;

    [SerializeField]
    private GameObject roomMenuContainer;

    [SerializeField]
    private TMPro.TextMeshProUGUI roomMenuNameText;

    [SerializeField]
    private GameObject lobbyMenuContainer;


    [Header("PC Configuration")]

    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private Camera pcCamera;

    [SerializeField]
    private GameObject cylinderMenuCanvas;

    [SerializeField]
    private Canvas menuCanvas;



    private PhotonView _photonView;


    private void Awake()
    {
        // load a new scene on all clients
        PhotonNetwork.AutomaticallySyncScene = true;
        _photonView = GetComponent<PhotonView>();

#if !UNITY_EDITOR && !DEBUG
        if (!Application.isMobilePlatform)
        {
            // configure scene for PC
            menuCanvas.transform.parent = null;
            menuCanvas.transform.position = new Vector3(0, 1.2f, 1);
            deactivateObjects.ForEach(o => o.SetActive(false));
            pcCamera.gameObject.SetActive(true);
            //cylinderMenuCanvas.GetComponent<MeshRenderer>().enabled = false;
            menuCanvas.worldCamera = pcCamera;
            menuCanvas.gameObject.SetActive(false);
        }
#endif
    }

    void Start()
    {
        if (!DEBUG)
        {
            // just make sure, menu is disabled and cutscene is enabled and played
            menu.SetActive(false);
            GameObjectExtensions.FindObject("MenuCorridor").SetActive(true);
            StartCoroutine(Intro());
        }
        ConnectToPhoton();
    }


    void Update()
    {
        // DEBUG START
        if (DEBUG)
        {
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
    }

    /// <summary>
    /// Short cutscene. Afterwards show menu and connect to photon.
    /// </summary>
    public IEnumerator Intro()
    {
        intro.Play();
        while (intro.isPlaying)
        {
            yield return new WaitForSeconds(1);
        }
#if !UNITY_EDITOR
        if (Application.isMobilePlatform) // vr
            menu.SetActive(true);
        else // pc
            menuCanvas.gameObject.SetActive(true);
#else
        menu.SetActive(true);
#endif
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
        PhotonNetwork.JoinLobby();
    }

    public void OnClickedPlayButton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            infoTextPanel.WriteLine("We are connecting to the network. Please wait.");
            return;
        }

        lobbyMenuContainer.SetActive(true);
        mainMenuContainer.SetActive(false);
    }

    public void OnClickedStartButton()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
        {
            infoTextPanel.WriteLine("We need support by another player.");
            return;
        }
        // start game
        if (!PhotonNetwork.IsMasterClient)
            infoTextPanel.WriteLine("Sorry, only your boss is allowed to start the game.");
        else
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public void OnClickedQuitButton()
    {
        Application.Quit();
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
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
            infoTextPanel.WriteLine("Create debug room...");
        }
        else
        {
            // load game scene for VR
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    /// <summary>
    /// Activate room menu and set status lights.
    /// </summary>
    public override void OnJoinedRoom()
    {
        infoTextPanel.WriteLine("Joined room \"" + PhotonNetwork.CurrentRoom.Name + "\", current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        roomMenuNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomMenuContainer.SetActive(true);
        playerReadyPanel.SetGreen(Application.isMobilePlatform ? GUIConstants.IndicatorLight.OPERATOR : GUIConstants.IndicatorLight.ASSISTANT); // set myself ready
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            playerReadyPanel.SetGreen(Application.isMobilePlatform ? GUIConstants.IndicatorLight.ASSISTANT : GUIConstants.IndicatorLight.OPERATOR);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not called if I am joining myself
        infoTextPanel.WriteLine("Player joined room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        playerReadyPanel.SetGreen(Application.isMobilePlatform ? GUIConstants.IndicatorLight.ASSISTANT : GUIConstants.IndicatorLight.OPERATOR);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        infoTextPanel.WriteLine("Player left room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        playerReadyPanel.SetRed(Application.isMobilePlatform ? GUIConstants.IndicatorLight.ASSISTANT : GUIConstants.IndicatorLight.OPERATOR);
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
