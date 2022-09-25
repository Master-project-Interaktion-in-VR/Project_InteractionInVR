using Photon.Pun;
using UnityEngine;

/// <summary>
/// Make the (Photon) owner of an object deal with gravity calculations.
/// Disables gravity if not the owner.
/// Otherwise weird effects occur.
/// </summary>
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
        if (_rigidbody != null)
            _rigidbody.isKinematic = !_photonView.IsMine;
    }
}
