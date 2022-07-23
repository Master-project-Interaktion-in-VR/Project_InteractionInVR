#define JOIN_TEST_ROOM
#define ON_OCULUS_LINK

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using static BuildManager;

public class AssemblySceneManager : MonoBehaviourPunCallbacks
{
    public static bool RUNNING_IN_TEST_ROOM;

    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private GameObject pcPlayerGui;

    [SerializeField]
    private List<GameObject> glyphSlots;

    [SerializeField]
    private List<Sprite> glyphSprites;

    [Header("Debug")]
    [SerializeField]
    private AssemblySuccessUnityEvent dummyEvent = new AssemblySuccessUnityEvent();


    private PhotonView _photonView;
    private PC_GUI_Manager _pcGuiManager;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("MyTestRoom", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Test room");
    }


    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _pcGuiManager = pcPlayerGui.GetComponent<PC_GUI_Manager>();

#if JOIN_TEST_ROOM
        if (!PhotonNetwork.IsConnected)
        {
            RUNNING_IN_TEST_ROOM = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        // invoke assembly success event for testing
        StartCoroutine(PublishDummyEvent(5));
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
    }

    private void ConfigurePcView()
    {
        // run this code for PC view

        pcPlayerGui.SetActive(true);

        foreach (GameObject obj in deactivateObjects)
        {
            obj.SetActive(false);
        }
        GameObject.Find("AvatarVrRigForMrtk").transform.Find("Head").GetComponent<TrackedPoseDriver>().enabled = false;
    }


    public void OnAssemblySuccess(bool success)
    {
        StartPuzzleVR();
    }

    public void StartPuzzleVR()
    {
        // generate an array with 4 fields filled with random numbers between 0 and 8
        int[] solution = new int[4];
        for (int i = 0; i < solution.Length; i++)
        {
            solution[i] = Random.Range(0, 9);

            glyphSlots[i].GetComponent<Image>().sprite = glyphSprites[solution[i]];
        }

        // send the solution to the other players
        _photonView.RPC("StartPuzzleRpc", RpcTarget.All, solution);
    }

    [PunRPC]
    public void StartPuzzleRpc(int[] solution)
    {
        if (_pcGuiManager != null) // why??
        {
            StartCoroutine(_pcGuiManager.StartPuzzle(solution));
        }
        else
        {
            Debug.Log("PC_GUI_Manager is null");
        }

    }

    [PunRPC]
    public void PuzzleSolved()
    {
        // TODO: What happens after the puzzle is solved?

    }








    private IEnumerator PublishDummyEvent(float delay)
    {
        yield return new WaitForSeconds(delay);
        dummyEvent.Invoke(true);
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
