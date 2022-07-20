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

    public void InitHoldingBody()
    {
        _photonView.RPC("InitHoldingBodyRpc", RpcTarget.All);
    }

    [PunRPC]
    private void InitHoldingBodyRpc()
    {
        gameObject.name = "holdingBody";
        AddComponentsRpc();
        // add gravity script after rigidbody has been added
        gameObject.AddComponent<DisableMyGravity>();
    }

    public void AddComponents()
    {
        _photonView.RPC("AddComponentsRpc", RpcTarget.All);
    }

    /// <summary>
    /// Used for holding body as well as antenna pieces.
    /// </summary>
    /// <returns> same GameObject with added components</returns>
    [PunRPC]
    public void AddComponentsRpc()
    {
        // add rigidBody to Object
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<PhotonTransformView>();
        //obj.AddComponent<PhotonView>();
        //obj.AddComponent<PhotonTransformView>();
        //obj.AddComponent<PhotonRigidbodyView>();
        // add ObjectManipulator to object
        Microsoft.MixedReality.Toolkit.UI.ObjectManipulator om = gameObject.AddComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
        om.TwoHandedManipulationType = Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Move | Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Rotate;
        om.AllowFarManipulation = false;
        // add Collision Manager
        //obj.AddComponent<CollisionManager>();
        // add Build tag
        //obj.tag = "BuildObject";
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
            // remove tag
            //obj.tag = "InitialObject";
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
