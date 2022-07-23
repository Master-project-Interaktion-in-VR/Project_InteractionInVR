using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMyGravity : MonoBehaviour
{
    private PhotonView _photonView;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.isKinematic = !_photonView.IsMine;
    }
}
