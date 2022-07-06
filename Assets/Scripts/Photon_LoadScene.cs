using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photon_LoadScene : MonoBehaviour
{
    public string GameScene_name;

    public void LoadScene()
    {
        PhotonNetwork.LoadLevel(GameScene_name);
    }
}
