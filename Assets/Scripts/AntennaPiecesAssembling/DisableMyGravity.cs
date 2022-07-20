using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMyGravity : MonoBehaviour
{
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (!_photonView.IsMine)
        {
            rigidbody.isKinematic = true;
        }
    }
}
