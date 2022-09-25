using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Photon helper to take over ownership of items as VR player.
/// </summary>
public class TakeoverOwnership : MonoBehaviour
{
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // request ownership of collider object if VR player touches them to be able to move them over the network
        if (other.gameObject.layer == LayerMask.NameToLayer("VrHands") && !_photonView.IsMine)
        {
            // this is from the perspective of the item
            GetComponent<PhotonView>().RequestOwnership();
        }
    }
}
