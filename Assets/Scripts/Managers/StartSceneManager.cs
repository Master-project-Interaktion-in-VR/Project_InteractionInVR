//#define VR_IN_EDITOR
//#define SKIP_INTRO


using System.Collections.Generic;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Set up scene for either VR or PC. 
/// Connect to Photon. Menu navigation.
/// Use compile-time constants to control VR or PC setup in the editor.
/// </summary>
public class StartSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string gameVersion;

    [SerializeField]
    private string gameSceneName;

    [SerializeField]
    private VideoPlayer introVideo;

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

    [SerializeField]
    private FadeScreen fadeScreen;


    private PhotonView _photonView;
    private bool _controllersNeeded;

    // Oculus integration controllers
    private bool _buttonTriggered;
    private InputDevice _rightHandedController;
    private InputDevice _leftHandedController;
    

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _photonView = GetComponent<PhotonView>();

#if !UNITY_EDITOR || UNITY_EDITOR && !VR_IN_EDITOR
        if (!Application.isMobilePlatform)
        {
            // configure scene for PC
            menuCanvas.transform.parent = null;
            menuCanvas.transform.position = new Vector3(0, 1.2f, 1);
            deactivateObjects.ForEach(o => o.SetActive(false));
            pcCamera.gameObject.SetActive(true);
            menuCanvas.worldCamera = pcCamera;
            menuCanvas.gameObject.SetActive(false);
            menuCanvas.transform.position = new Vector3(-0.449f, 3.465f, 0.804f);
            menuCanvas.transform.rotation = Quaternion.Euler(new Vector3(0f, -18.4f, 0f));
        }
#endif
    }

    void Start()
    {
		TryInitialize();

#if UNITY_EDITOR && SKIP_INTRO // skip intro and directly connect to photon
        ConnectToPhoton();
#if VR_IN_EDITOR // decide which menu to show
        menu.SetActive(true);
#else
        menuCanvas.gameObject.SetActive(true);
#endif

#else // play intro if not coming from ingame menu
        if (!SceneSpanningData.isComingFromGame)
        {
            // just make sure, menu is disabled and cutscene is enabled and played
            menu.SetActive(false);
            StartCoroutine(Intro());
        }
        else
        {
            menu.SetActive(true);
            ConnectToPhoton();
        }
#endif
    }

    private void Update()
    {
        // skip intro functionality with keyboard
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                Debug.Log("Skipped Intro Video");
                StopVideoPcShortcut();
            }
        }

#if UNITY_EDITOR && VR_IN_EDITOR
        _controllersNeeded = true;
#elif !UNITY_EDITOR
        if (Application.isMobilePlatform)
        {
            _controllersNeeded = true;
        }
#endif

        if (_controllersNeeded)
        {
            // initialize controllers because they are not instantly available
            if (!_rightHandedController.isValid || !_leftHandedController.isValid)
            {
                TryInitialize();
            }
            else
            {
                // skip intro in VR
                _leftHandedController.TryGetFeatureValue(CommonUsages.menuButton, out var rightTriggerValue);
                if (rightTriggerValue && !_buttonTriggered)
                {
                    VideoStopper();
                    _buttonTriggered = true;
                }
                else if (!rightTriggerValue && _buttonTriggered)
                {
                    CancelVideoStopper();
                    _buttonTriggered = false;
                }
            }
        }

#if UNITY_EDITOR // debug button for joining a room quickly
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
#endif
    }

    /// <summary>
    /// Short cutscene. Afterwards show menu and connect to photon.
    /// </summary>
    public IEnumerator Intro()
    {
        introVideo.Play();    
        yield return new WaitForSeconds(1);

        while (introVideo.isPlaying)
        {
            yield return new WaitForSeconds(1);
        }
#if !UNITY_EDITOR
        if (Application.isMobilePlatform) // vr
            menu.SetActive(true);
        else // pc
            menuCanvas.gameObject.SetActive(true);
#else
#if VR_IN_EDITOR
        menu.SetActive(true);
#else
        menuCanvas.gameObject.SetActive(true);
#endif
#endif
        yield return new WaitForSeconds(1);
        ConnectToPhoton();
    }

    /// <summary>
    /// Stop video by holding Back button for 3 seconds.
    /// </summary>
    private void VideoStopper()
    {
        Invoke(nameof(StopVideo), 3);
    }

    /// <summary>
    /// On Back button up. Skip was aborted.
    /// </summary>
    private void CancelVideoStopper()
    {
        CancelInvoke(nameof(StopVideo));
    }

    /// <summary>
    /// Skip intro and connect to Photon.
    /// </summary>
    private void StopVideo()
    {
        introVideo.Stop();
        menu.SetActive(true);
        ConnectToPhoton();
    }

    /// <summary>
    /// Skip intro on PC.
    /// </summary>
    public void StopVideoPcShortcut()
    {
        introVideo.Stop();
        menuCanvas.gameObject.SetActive(true);
        ConnectToPhoton(); 
    }

    /// <summary>
    /// Connect to Photon master.
    /// </summary>
    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    /// <summary>
    /// Photon callback for established connection.
    /// Join the lobby if connected.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        infoTextPanel.WriteLine("Connected to Photon master. We are online!");
        lightsPanel.SetGreen(GUIConstants.IndicatorLight.PHOTON);
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Menu navigation.
    /// </summary>
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

    /// <summary>
    /// Menu navigation.
    /// </summary>
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
            StartCoroutine(LoadGame());
        }
    }

    /// <summary>
    /// Menu navigation.
    /// </summary>
    public void OnClickedQuitButton()
    {
        Application.Quit();
    }

    /// <summary>
    /// Load game scene.
    /// </summary>
    private IEnumerator LoadGame()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        PhotonNetwork.LoadLevel(gameSceneName);
    }

    /// <summary>
    /// Photon callback.
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

    /// <summary>
    /// Photon callback.
    /// </summary>
    public override void OnPlayerEnteredRoom(Player other)
    {
        // not called if I am joining myself
        infoTextPanel.WriteLine("Player joined room, current players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        playerReadyPanel.SetGreen(Application.isMobilePlatform ? GUIConstants.IndicatorLight.ASSISTANT : GUIConstants.IndicatorLight.OPERATOR);
    }

    /// <summary>
    /// Photon callback.
    /// </summary>
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
    
    private void TryInitialize()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach (InputDevice device in allDevices)
        {
            if (device.name.Contains("Right"))
            {
                _rightHandedController = device;
            }
            else if (device.name.Contains("Left"))
            {
                _leftHandedController = device;
            }
        }
    }

    /// <summary>
    /// Debug button was clicked. Join test room. Load next scene.
    /// </summary>
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
            PhotonNetwork.LoadLevel(gameSceneName);
        }
    }
}
