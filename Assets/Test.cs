using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviourPun
{

    private PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        _photonView.RPC("TestPrintRpc", RpcTarget.Others, (object)"test");
    }

    [PunRPC]
    private void TestPrintRpc(string test)
    {
        Debug.LogError(test);
    }
}
