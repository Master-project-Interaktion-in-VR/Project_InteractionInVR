using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Item : MonoBehaviour
{
    [SerializeField] private bool resetToOrigin;
    [SerializeField] private float resetDelayTime;
    
    private Renderer _renderer;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rigidbody;
    private InventoryManager _inventoryManager;
    
    private Vector3 _originPos;
    private Quaternion _originRotation;
    
    private bool _colliderTriggered;
    private bool _selected;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _grabInteractable = GetComponentInParent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();
        _inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
        _originPos = transform.position;
        _originRotation = transform.rotation;
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

    private void OnSelectEnter(SelectEnterEventArgs arg0)
    {
        CancelInvoke(nameof(ResetToOrigin));
        _selected = true;
        _renderer.material.DisableKeyword("_EMISSION");
    }


    private void OnSelectExit(SelectExitEventArgs arg0)
    {
        Invoke(nameof(ResetToOrigin), resetDelayTime);
        _selected = false;
    }

    private void ResetToOrigin()
    {
        if (resetToOrigin && !_colliderTriggered && !_selected)
        {
            //transform.rotation = _originRotation;
            //transform.position = _originPos;
            //_rigidbody.velocity = Vector3.zero;
            NetworkHelper networkHelper = GetComponent<NetworkHelper>();
            networkHelper.SetPositionAll(_originPos, _originRotation.eulerAngles);
            networkHelper.SetRigidbodyVelocity(Vector3.zero);
        }
    }

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
    
    public void HoverExit(HoverExitEventArgs arg0)
    {
        foreach (var material in _renderer.materials)
        {
            material.DisableKeyword("_EMISSION");
        }
    }

    public void RevealItem(SelectEnterEventArgs arg0)
    {
        gameObject.layer = 0;
    }
    
    public void ActivateEnter(ActivateEventArgs arg0)
    {
        if(_inventoryManager != null)
            _inventoryManager.PutItemInInventory(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        SetColliderTriggered(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        SetColliderTriggered(other);
        
        if (other.CompareTag("Origin"))
        {
            Invoke(nameof(ResetToOrigin), resetDelayTime);
        }
    }

    private void SetColliderTriggered(Collider other)
    {
        _colliderTriggered = other.gameObject.GetComponent<XRBaseController>() != null;
    }
}
