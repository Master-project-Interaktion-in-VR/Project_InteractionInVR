using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTIIIIII : MonoBehaviour
{
    private PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private bool _abc;
    private bool _xyz;

    // Update is called once per frame
    void Update()
    {
        if (_xyz)
        {
            _xyz = false;
            if (PhotonNetwork.IsMasterClient)
                TESTA();
        }
        if (_abc || !PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount < 2)
            return;
        _abc = true;
        StartCoroutine(Test());
    }
    private IEnumerator Test()
    {
        Debug.LogError("wait 3 seconds");
        yield return new WaitForSeconds(3);
        _xyz = true;
    }

    [PunRPC]
    public void TESTA()
    {
        Debug.Log("SAJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
    }
}
