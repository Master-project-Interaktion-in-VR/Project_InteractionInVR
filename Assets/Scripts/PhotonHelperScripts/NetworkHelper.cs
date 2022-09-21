using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provide useful Photon functions.
/// Can be used for multiple types of items and objects.
/// </summary>
public class NetworkHelper : MonoBehaviour
{
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Set gravity of an object's rigidbody.
    /// </summary>
    public void SetGravity(bool gravity)
    {
        _photonView.RPC("SetGravityRpc", RpcTarget.All, gravity);
    }

    /// <summary>
    /// Corresponding RPC for setting gravity.
    /// </summary>
    [PunRPC]
    public void SetGravityRpc(bool gravity)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.useGravity = gravity;
        }
    }

    /// <summary>
    /// Set the parent of an object.
    /// </summary>
    public void SetParent(Transform parent)
    {
        _photonView.RPC("SetParentRpc", RpcTarget.All, parent.name);
    }
    /// <summary>
    /// Move object to scene root.
    /// </summary>
    public void SetParentRoot()
    {
        _photonView.RPC("SetParentRootRpc", RpcTarget.All);
    }
    
    /// <summary>
    /// Change the name of an object.
    /// </summary>
    public void SetName(string name)
    {
        _photonView.RPC("SetNameRpc", RpcTarget.All, name);
    }

    /// <summary>
    /// Corresponding RPC for setting the name.
    /// </summary>
    [PunRPC]
    public void SetNameRpc(string name)
    {
        transform.name = name;
    }

    /// <summary>
    /// Corresponding RPC for setting the parent by name.
    /// </summary>
    [PunRPC]
    private void SetParentRpc(string parentName)
    {
        transform.SetParent(GameObject.Find(parentName).transform);
    }

    /// <summary>
    /// Corresponding RPC for moving the object to scene root.
    /// </summary>
    [PunRPC]
    private void SetParentRootRpc()
    {
        transform.SetParent(null);
    }

    /// <summary>
    /// Assembly helper function.
    /// </summary>
    public void RemoveComponents()
    {
        _photonView.RPC("RemoveComponentsRpc", RpcTarget.All);
    }

    /// <summary>
    /// Remove the components of the single object which is now assembled in a holdingBody
    /// </summary>
    /// <returns>same GameObject with less components</returns>
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

    /// <summary>
    /// Set the position and rotation of an object.
    /// </summary>
    public void SetPosition(Transform transform)
    {
        _photonView.RPC("SetPositionRpc", RpcTarget.Others, transform.position, transform.rotation.eulerAngles);
    }

    /// <summary>
    /// Corresponding RPC for setting an object's position and rotation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    [PunRPC]
    private void SetPositionRpc(Vector3 position, Vector3 rotation)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
