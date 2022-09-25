using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Manage room creation, joining, and filtering.
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ListManager roomListManager;

    [SerializeField]
    private List<string> roomNames;

    [SerializeField]
    private GameObject lobbyMenuContainer;


    private bool _platformMobile;
    private List<string> _joinableRooms;
    private List<string> _closedRooms;
    private bool _roomsChanged;

    private void Awake()
    {
        _platformMobile = Application.isMobilePlatform;
    }

    private void Update()
    {
        if (roomListManager.isActiveAndEnabled && _roomsChanged)
        {
            _roomsChanged = false;
            // we are in the lobby menu, subscribe to click events
            Dictionary<string, Button> addedItems = roomListManager.Refresh(_joinableRooms);
            foreach (KeyValuePair<string, Button> pair in addedItems)
            {
                pair.Value.onClick.AddListener(delegate { JoinRoom(pair.Key); });
            }
            _closedRooms.ForEach(room => roomListManager.RemoveItem(room));
        }

    }

    /// <summary>
    /// Photon callback for rooms.
    /// Only contains the newly added rooms. If a room is closed, an update containing 
    /// the room with null values in it and RemovedFromList == true is sent by Photon.
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("ROOM LIST " + roomList.Count);
        base.OnRoomListUpdate(roomList);

        // remove all room names that are already used
        List<string> runningRooms = roomList.Select(roomInfo => roomInfo.Name).ToList();
        roomNames.RemoveAll(roomName => runningRooms.Contains(roomName));

        // filter rooms that have a free slot for my platform
        _joinableRooms = roomList.Where(room => !room.RemovedFromList)
            .Where(room => room.CustomProperties["n"].ToString().Equals(_platformMobile ? GUIConstants.PLATFORM_VR : GUIConstants.PLATFORM_PC)) // _platformMobile ? GUIConstants.PLATFORM_VR : GUIConstants.PLATFORM_PC
            .Select(room => room.Name)
            .ToList();
        // filter closed rooms if this update closed a room
        _closedRooms = roomList.Where(room => room.RemovedFromList).Select(room => room.Name).ToList();
        _roomsChanged = true;
    }

    /// <summary>
    /// Create a new room with custom properties.
    /// </summary>
    public void CreateRoom()
    {
        lobbyMenuContainer.SetActive(false);
        RoomOptions roomOptions = new RoomOptions();
        //roomOptions.IsOpen = true;
        //roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        roomOptions.EmptyRoomTtl = 500;
        roomOptions.CustomRoomProperties = new Hashtable();
        // indicate needed platform:
        roomOptions.CustomRoomProperties.Add("n", _platformMobile ? GUIConstants.PLATFORM_PC : GUIConstants.PLATFORM_VR); // n for need

        string[] customLobbyProperties = new string[] { "n" };
        roomOptions.CustomRoomPropertiesForLobby = customLobbyProperties;

        int randIndex = Random.Range(0, roomNames.Count);
        PhotonNetwork.CreateRoom(roomNames[randIndex], roomOptions);
        roomNames.Remove(roomNames[randIndex]);
    }

    /// <summary>
    /// Join an existing room by room name.
    /// </summary>
    private void JoinRoom(string roomName)
    {
        lobbyMenuContainer.SetActive(false);
        PhotonNetwork.JoinRoom(roomName);
    }
}
