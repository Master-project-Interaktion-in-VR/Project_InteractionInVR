#define JOIN_TEST_ROOM
#define ON_OCULUS_LINK

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XDPaint.Controllers;

/// <summary>
/// Set up scene for either VR or PC. 
/// Scene skipping for PC player.
/// Use compile-time constants to control VR or PC setup in the editor and test rooms.
/// </summary>
public class EnvironmentGameSceneManager : MonoBehaviourPunCallbacks
{
    public static bool RUNNING_IN_TEST_ROOM;

    [Header("PC Configuration")]

    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private GameObject vrCamera;

    [SerializeField]
    private GameObject pcPlayerGui;

    [SerializeField]
    private InputController paintInputController;

    [SerializeField] 
    private GameObject minimapCamera;


    private PhotonView _photonView;


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();

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
        else 
        {
            minimapCamera.SetActive(false);
        }
#endif
    }

    private void Update()
    {
        // skip scene only on PC
        if (Application.isMobilePlatform)
            return;
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                Debug.Log("EnvironmentScene skipped");
                StartCoroutine(LoadAssemblyScene());
            }
        }
    }

    #region Test room Photon callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("SOME_RANDOM_NAME_21398", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Test room");
    }
    #endregion

    /// <summary>
    /// Configure scene for PC player.
    /// </summary>
    private void ConfigurePcView()
    {
        pcPlayerGui.SetActive(true);

        // disable VR components
        Destroy(vrCamera.GetComponent<TrackedPoseDriver>());
        //Destroy(vrCamera.GetComponent<AudioListener>());
        // do not destroy camera because of paint scripts
        vrCamera.GetComponent<Camera>().cullingMask = 0;

        // deactivate hand models
        foreach (GameObject obj in deactivateObjects)
        {
            obj.SetActive(false);
        }
        paintInputController.enabled = false;
    }

    private IEnumerator LoadAssemblyScene()
    {
        //fadeScreen.FadeOut();
        //yield return new WaitForSeconds(fadeScreen.fadeDuration);

        _photonView.RPC("LoadScene", RpcTarget.All, "AssemblyScene");
        yield return null;
    }

    /// <summary>
    /// Load scene RPC.
    /// Scene loading only allowed for master client.
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
    /// Is the game currently running on VR glasses?
    /// </summary>
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
    }
}
