using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoverOwnership : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // request ownership of collider object if VR player touches them to be able to move them over the network
        if (other.gameObject.layer == LayerMask.NameToLayer("VrHands"))
            GetComponent<PhotonView>().RequestOwnership();
    }
}
