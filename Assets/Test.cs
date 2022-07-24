using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
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
        _photonView.RPC("TestPrintRpc", RpcTarget.All, "test");
    }

    [PunRPC]
    private void TestPrintRpc(string test)
    {
        Debug.Log(test);
    }
}
