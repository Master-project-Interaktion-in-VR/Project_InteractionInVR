using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Item : MonoBehaviour
{
    [SerializeField] 
    private bool resetToOrigin;
    
    [SerializeField] 
    private float resetDelayTime;
    
    [SerializeField] 
    private int itemIndex;
    
    [SerializeField] 
    private Inventory inventory;
    
    
    private Renderer _renderer;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rigidbody;
    private InventoryManager _inventoryManager;
    private PhotonView _photonView;
    
    private Vector3 _originPos;
    private Quaternion _originRotation;
    
    private bool _colliderTriggered;
    private bool _selected;
    private bool _collected;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _grabInteractable = GetComponentInParent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();
        _inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
        _photonView = GetComponent<PhotonView>();
        
        var myTransform = transform;
        _originPos = myTransform.position;
        _originRotation = myTransform.rotation;
    }

    private void OnEnable()
    {
        if(_grabInteractable != null)
        {
            _grabInteractable.selectEntered.AddListener(OnSelectEnter);
            _grabInteractable.selectExited.AddListener(OnSelectExit);
        }
    }

    private void OnDisable()
    {
        if(_grabInteractable != null)
        {
            _grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
            _grabInteractable.selectExited.RemoveListener(OnSelectExit);
        }
    }
    
    /// <summary>
    /// ResetToOrigin for photon
    /// </summary>
    [PunRPC]
    public void ResetToOriginRpc()
    {
        transform.rotation = _originRotation;
        transform.position = _originPos;
        _rigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// Highlights item material with _EMISSION property color, while hovering with hands
    /// </summary>
    /// <param name="arg0"></param>
    public void HoverEnter(HoverEnterEventArgs arg0)
    {
        if (!_selected)
        {
            foreach (var material in _renderer.materials)
            {
                material.EnableKeyword("_EMISSION");
            }
        }
    }
    
    /// <summary>
    /// Highlights item material with _EMISSION property color, while hovering with hands
    /// </summary>
    /// <param name="arg0"></param>
    public void HoverExit(HoverExitEventArgs arg0)
    {
        foreach (var material in _renderer.materials)
        {
            material.DisableKeyword("_EMISSION");
        }
    }

    /// <summary>
    /// Makes item visible when selected, by setting its layer to default
    /// </summary>
    /// <param name="arg0"></param>
    public void RevealItem(SelectEnterEventArgs arg0)
    {
        gameObject.layer = 0;
    }
    
    /// <summary>
    /// Called when primary button is pressed while holding an item
    /// Calls the inventory method to destroy item and show in UI
    /// </summary>
    /// <param name="arg0"></param>
    public void ActivateEnter(ActivateEventArgs arg0)
    {
        if (_inventoryManager != null && inventory != null && !_collected)
        {
            inventory.collectedAntennaParts[itemIndex] = true; // Set scriptable object, to find out which items is collected
            _collected = true;
            StartCoroutine(_inventoryManager.PutItemInInventory(gameObject));
        }
    }

    /// <summary>
    /// When item is selected, cancel resetting to origin and remove highlighting material
    /// </summary>
    /// <param name="arg0"></param>
    private void OnSelectEnter(SelectEnterEventArgs arg0)
    {
        CancelInvoke(nameof(ResetToOrigin));
        _selected = true;
        _renderer.material.DisableKeyword("_EMISSION");
    }

    /// <summary>
    /// When item is released, invoke ResetToOrigin after resetDelayTime
    /// </summary>
    /// <param name="arg0"></param>
    private void OnSelectExit(SelectExitEventArgs arg0)
    {
        Invoke(nameof(ResetToOrigin), resetDelayTime);
        _selected = false;
    }

    /// <summary>
    /// Resets item to origin position, so it doesn't get lost
    /// </summary>
    private void ResetToOrigin()
    {
        if (resetToOrigin && !_colliderTriggered && !_selected && _photonView.IsMine) // only reset to origin if this is my photon view (VR)
        {
            _photonView.RPC("ResetToOriginRpc", RpcTarget.All);
        }
    }

    /// <summary>
    /// Item collider is triggered
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        SetColliderTriggered(other);
    }
    
    /// <summary>
    /// Item collider is exited
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        SetColliderTriggered(other);
        
        // reset item to origin if it exits origin collider
        if (other.CompareTag("Origin"))
        {
            Invoke(nameof(ResetToOrigin), resetDelayTime);
        }
    }

    /// <summary>
    /// Sets a boolean to check if a collider is triggered when resetting to origin
    /// </summary>
    /// <param name="other"></param>
    private void SetColliderTriggered(Collider other)
    {
        _colliderTriggered = other.gameObject.GetComponent<XRBaseController>() != null;
    }
}
