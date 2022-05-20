using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject playerPrefab;


    private void Start()
    {
        GameObject playerBrush = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 3, 0), Quaternion.identity);
    }
}
