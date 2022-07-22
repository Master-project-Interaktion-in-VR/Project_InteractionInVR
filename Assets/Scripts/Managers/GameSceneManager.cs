#define START_PHOTON
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    [Header("PC Configuration")]

    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private GameObject vrCamera;

    [SerializeField]
    private Camera assistantCamera;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("TEST", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Test room");
    }

    void Start()
    {
#if START_PHOTON

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
#endif

#if !UNITY_EDITOR

        if (!Application.isMobilePlatform)
        {
            // is assistant
            Destroy(vrCamera.GetComponent<TrackedPoseDriver>());
            Destroy(vrCamera.GetComponent<AudioListener>());
            // destroying camera causes paint exception, should we disable VR painting configurations for pc?
            vrCamera.GetComponent<Camera>().cullingMask = 0; // necessary?

            // leave all VR components, do we need to remove them?

            // deactivate hand models
            foreach (GameObject obj in deactivateObjects)
            {
                obj.SetActive(false);
            }
            assistantCamera.gameObject.SetActive(true);
        }
#endif
    }
}
