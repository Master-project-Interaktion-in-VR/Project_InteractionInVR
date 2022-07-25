using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BuildManager;

public class PhotonSceneLoader : MonoBehaviourPun
{
    [SerializeField]
    private AssemblySuccessUnityEvent asue = new AssemblySuccessUnityEvent();


    private Scene scene;
    PhotonView _photonView;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        _photonView = PhotonView.Get(this);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                if (scene.name == "StartScene")
                {
                    GameObject.Find("Launcher").GetComponent<Launcher>().StopVideoPcShortcut();
                }
                if (scene.name == "EnvironmentGameScene")
                {
                    Debug.Log("EnvironmentScene skipped");
                    //SceneManager.LoadScene("AssemblyScene");
                    _photonView.RPC("LoadScene", RpcTarget.All, "AssemblyScene");
                }
                else if (scene.name == "AssemblyScene")
                {
                    if (GameObject.Find("Puzzle") == null)
                    {
                        Debug.Log("Skip assembly");
                        asue.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("Skip puzzle");
                        _photonView.RPC("LoadScene", RpcTarget.All, "EndScene");
                    }
                }
            }
        }
    }

    [PunRPC]
    public void LoadScene(string GameScene_name)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(GameScene_name);
        }
    }
}
