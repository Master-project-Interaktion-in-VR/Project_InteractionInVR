#define JOIN_TEST_ROOM
#define ON_OCULUS_LINK

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using static BuildManager;

/// <summary>
/// Set up scene for either VR or PC. 
/// Assembly and puzzle skipping for PC player.
/// Use compile-time constants to control VR or PC setup in the editor and test rooms.
/// </summary>
public class AssemblySceneManager : MonoBehaviourPunCallbacks
{
    public static bool RUNNING_IN_TEST_ROOM;

    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private GameObject pcPlayerGui;

    [SerializeField]
    private PhotonView table;

    [SerializeField]
    private GameObject puzzlePlane;

    [SerializeField]
    private GameObject drawingEraseButton;

    [SerializeField]
    private List<GameObject> glyphSlots;

    [SerializeField]
    private List<Sprite> glyphSprites;
    
    [SerializeField] 
    private FadeScreen fadeScreen;

    [Header("Debug")]
    [SerializeField]
    private AssemblySuccessUnityEvent assemblySuccessDebugEvent = new AssemblySuccessUnityEvent();
    

    private PhotonView _photonView;
    private PC_GUI_Manager _pcGuiManager;


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _pcGuiManager = pcPlayerGui.GetComponent<PC_GUI_Manager>();


#if JOIN_TEST_ROOM
        if (!PhotonNetwork.IsConnected)
        {
            RUNNING_IN_TEST_ROOM = true;
            PhotonNetwork.ConnectUsingSettings();
        }
#endif

#if UNITY_EDITOR && !ON_OCULUS_LINK
        ConfigurePcView();
#endif
#if !UNITY_EDITOR
        if (!Application.isMobilePlatform)
        {
            ConfigurePcView();
        }
        else {
            table.RequestOwnership();
        }
#endif
    }

    private void Update()
    {
        // skipping only for PC
        if (Application.isMobilePlatform)
            return;
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                if (GameObject.Find("Puzzle") == null)
                {
                    Debug.Log("Skip assembly");
                    assemblySuccessDebugEvent.Invoke(true);
                }
                else
                {
                    Debug.Log("Skip puzzle");
                    StartCoroutine(LoadEndScene());
                }
            }
        }
    }

    #region Test room Photon callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("MyTestRoomdd", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Test room");
    }
    #endregion

    private IEnumerator LoadEndScene()
    {
        //fadeScreen.FadeOut();
        //yield return new WaitForSeconds(fadeScreen.fadeDuration);

        _photonView.RPC("LoadScene", RpcTarget.All, "EndScene");
        yield return null;
    }

    /// <summary>
    /// Scene loading only on master client.
    /// </summary>
    [PunRPC]
    public void LoadScene(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(sceneName);
        }
    }

    /// <summary>
    /// Configure scene for PC player.
    /// </summary>
    private void ConfigurePcView()
    {
        pcPlayerGui.SetActive(true);

        foreach (GameObject obj in deactivateObjects)
        {
            obj.SetActive(false);
        }
        // disable head tracker
        GameObject.Find("AvatarVrRigForMrtk").transform.Find("Head").GetComponent<TrackedPoseDriver>().enabled = false;
    }

    /// <summary>
    /// Assembly of the antenna was successful.
    /// Start the puzzle.
    /// </summary>
    public void OnAssemblySuccess(bool success)
    {
        StartPuzzleVR();
    }

    /// <summary>
    /// Skip assembly.
    /// </summary>
    public void OnAssemblySuccessPcShortcut(bool success)
    {
        _photonView.RPC("StartPuzzleVR", RpcTarget.Others);
    }

    /// <summary>
    /// Start puzzle RPC. Executed on VR.
    /// Show glyphs and enable drawing screen.
    /// </summary>
    [PunRPC]
    public void StartPuzzleVR()
    {
        drawingEraseButton.SetActive(true);
        // generate an array with 4 fields filled with random numbers between 0 and 8
        int[] solution = new int[4];
        for (int i = 0; i < solution.Length; i++)
        {
            solution[i] = Random.Range(0, 9);

            glyphSlots[i].GetComponent<Image>().sprite = glyphSprites[solution[i]];
        }

        puzzlePlane.SetActive(true);

        // send the solution to the other players
        _photonView.RPC("StartPuzzlePcRpc", RpcTarget.Others, solution);
    }

    /// <summary>
    /// Start puzzle RPC. Executed on PC.
    /// Enable puzzle GUI.
    /// </summary>
    [PunRPC]
    public void StartPuzzlePcRpc(int[] solution)
    {
        if (_pcGuiManager != null) // why??
        {
            drawingEraseButton.SetActive(true);
            StartCoroutine(_pcGuiManager.StartPuzzle(solution));
        }
        else
        {
            Debug.Log("PC_GUI_Manager is null");
        }
    }

    /// <summary>
    /// Puzzle success event.
    /// </summary>
    /// <param name="success">Boolean for inspector convenience</param>
    public void OnPuzzleSuccess(bool success)
    {
        StartCoroutine(PuzzleSuccess());
    }

    /// <summary>
    /// Puzzle was solved successfully.
    /// </summary>
    private IEnumerator PuzzleSuccess()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        
        _photonView.RPC("PuzzleSolved", RpcTarget.All);
    }

    /// <summary>
    /// Puzzle was solved successfully RPC.
    /// Load end scene if master.
    /// </summary>
    [PunRPC]
    public void PuzzleSolved()
    {
        Debug.Log("PUZZLE SOLVED");
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("EndScene");
    }








    private IEnumerator PublishDummyEvent(float delay)
    {
        yield return new WaitForSeconds(delay);
        assemblySuccessDebugEvent.Invoke(true);
    }



    public static bool IsRunningOnGlasses()
    {
#if UNITY_EDITOR && ON_OCULUS_LINK
        return true;
#endif
#if UNITY_EDITOR && !ON_OCULUS_LINK
        return false;
#endif
#if !UNITY_EDITOR
        return Application.isMobilePlatform;
#endif
        // OVRPlugin.SystemHeadset.Oculus_Link_Quest
    }
}
