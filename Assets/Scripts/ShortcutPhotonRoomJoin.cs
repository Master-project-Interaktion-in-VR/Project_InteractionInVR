using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildManager;

public class ShortcutPhotonRoomJoin : MonoBehaviourPunCallbacks
{
    public AssemblySuccessUnityEvent asue = new AssemblySuccessUnityEvent();
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
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        // invoke assembly success event for testing
        StartCoroutine(Cor());
    }

    private IEnumerator Cor()
    {
        yield return new WaitForSeconds(2);
        //if (Application.isMobilePlatform)
            asue.Invoke(true);
    }
}
