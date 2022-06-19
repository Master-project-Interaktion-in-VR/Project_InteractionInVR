using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ListManager roomListManager;

    [SerializeField]
    private List<string> roomNames;


    private bool _platformMobile;

    void Awake()
    {
        _platformMobile = Application.isMobilePlatform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if (!roomListManager.isActiveAndEnabled)
            return;

        // filter rooms that have a free slot for my platform
        List<string> joinableRooms = roomList.Where(room => room.CustomProperties["n"].ToString().Equals(_platformMobile ? GUIConstants.PLATFORM_VR : GUIConstants.PLATFORM_PC))
            .Select(room => room.Name)
            .ToList();

        // subscribe to click event
        Dictionary<string, Button> addedItems = roomListManager.Refresh(joinableRooms);
        foreach(KeyValuePair<string, Button> pair in addedItems)
        {
            pair.Value.onClick.AddListener(delegate { JoinRoom(pair.Key); });
        }
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        //roomOptions.IsOpen = true;
        //roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add("n", _platformMobile ? GUIConstants.PLATFORM_PC : GUIConstants.PLATFORM_VR);

        string[] customLobbyProperties = new string[] { "n" };
        roomOptions.CustomRoomPropertiesForLobby = customLobbyProperties;

        int randIndex = Random.Range(0, roomNames.Count - 1);
        PhotonNetwork.CreateRoom(roomNames[randIndex], roomOptions);
        roomNames.Remove(roomNames[randIndex]);
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
