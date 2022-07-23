#define JOIN_TEST_ROOM
#define ON_OCULUS_LINK

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XDPaint.Controllers;

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



    [Header("Vibration and Audio")]

    [SerializeField]
    private XRBaseController leftController;

    [SerializeField]
    private XRBaseController rightController;

    [SerializeField]
    private float vibrationDuration;

    [SerializeField]
    private float vibrationIntensity;

    [SerializeField]
    private InventoryManager InventoryManager;

    [SerializeField]
    private AudioSource detectorAudio;



    private PhotonView _photonView;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("DSDDHALD", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Test room");
    }

    void Awake()
    {
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
#endif
        // TODO request controllers ownership????
    }

    private void ConfigurePcView()
    {
        // run this for PC view
        // is assistant
        pcPlayerGui.SetActive(true);

        // disable VR components
        Destroy(vrCamera.GetComponent<TrackedPoseDriver>());
        //Destroy(vrCamera.GetComponent<AudioListener>());
        // destroying camera causes paint exception, should we disable VR painting configurations for pc?
        vrCamera.GetComponent<Camera>().cullingMask = 0; // necessary?

        // leave all VR components, do we need to remove them?

        // deactivate hand models
        foreach (GameObject obj in deactivateObjects)
        {
            obj.SetActive(false);
        }
        paintInputController.enabled = false;
    }


    [PunRPC]
    public void TriggerVibrationRpc()
    {
        if (InventoryManager.DetectorIsInLeftHand())
            leftController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
        else if (InventoryManager.DetectorIsInRightHand())
            rightController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
        else
            return;

        Debug.Log("Trigger vibration");
    }


    [PunRPC]
    public void TriggerSoundRpc()
    {
        Debug.Log("Trigger sound");
        detectorAudio.Play();
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
    }
}