using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PhotonSceneLoader : MonoBehaviour
{
    public void LoadScene(string GameScene_name)
    {
        PhotonNetwork.LoadLevel(GameScene_name);
    }
}
