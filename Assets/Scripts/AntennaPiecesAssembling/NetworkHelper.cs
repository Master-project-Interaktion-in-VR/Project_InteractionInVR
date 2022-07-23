using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void SetParent(Transform parent)
    {
        _photonView.RPC("SetParentRpc", RpcTarget.All, parent.name);
    }
    public void SetParentRoot()
    {
        _photonView.RPC("SetParentRootRpc", RpcTarget.All);
    }

    [PunRPC]
    private void SetParentRpc(string parentName)
    {
        transform.SetParent(GameObject.Find(parentName).transform);
    }

    [PunRPC]
    private void SetParentRootRpc()
    {
        transform.SetParent(null);
    }

    public void RemoveComponents()
    {
        _photonView.RPC("RemoveComponentsRpc", RpcTarget.All);
    }

    /// <summary>
    /// Removes the components of the single object which is now assembled in a holdingBody
    /// </summary>
    /// <returns>same GameObject with les components</returns>
    [PunRPC]
    private void RemoveComponentsRpc()
    {
        try
        {
            // remove RigidBody of object
            //Destroy(obj.AddComponent<PhotonView>());
            Destroy(GetComponent<PhotonTransformView>());
            //Destroy(obj.GetComponent<PhotonRigidbodyView>());
            Destroy(GetComponent<Rigidbody>());
            // remove ObjectManipulator of object
            Destroy(GetComponent<Microsoft.MixedReality.Toolkit.UI.CursorContextObjectManipulator>());
            Destroy(GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>());
            // remove Collision Manager
            //Destroy(obj.GetComponent<CollisionManager>());
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    public void SetPosition(Transform transform)
    {
        _photonView.RPC("SetPositionRpc", RpcTarget.Others, transform.position, transform.rotation.eulerAngles);
    }

    [PunRPC]
    private void SetPositionRpc(Vector3 position, Vector3 rotation)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
