using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonSceneLoader : MonoBehaviourPun
{
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
            if (Input.GetKey(KeyCode.T))
            {
                if (scene.name == "EnvironmentGameScene")
                {
                    Debug.Log("EnvironmentScene skipped");
                    //SceneManager.LoadScene("AssemblyScene");
                    _photonView.RPC("LoadScene", RpcTarget.All, "AssemblyScene");
                }
                else if (scene.name == "AssemblyScene")
                {
                    Debug.Log("AssemblyScene skipped");
                    _photonView.RPC("LoadScene", RpcTarget.All, "EndScene");
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
