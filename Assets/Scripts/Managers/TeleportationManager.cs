using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider provider;
    
    private InputAction _thumbstick;
    private bool _isActive;

    private void Start()
    {
        // Don't show ray until thumbstick is moved
        rayInteractor.enabled = false;
        
        // Get InputActions
        var activate = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Teleport Mode Activate");
        activate.Enable();
        activate.performed += OnTeleportActivate;
        
        var cancel = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Teleport Mode Cancel");
        cancel.Enable();
        cancel.performed += OnTeleportCancel;

        _thumbstick = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Move");
        _thumbstick.Enable();
    }

    private void Update()
    {
        // If thumbstick released
        if (_isActive && !_thumbstick.triggered)
        {
            // If ray hits something valid
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // Create a new TeleportationRequest with the hit point as his destinationPosition
                TeleportRequest request = new TeleportRequest()
                {
                    destinationPosition = hit.point,
                    //destinationRotation = ,
                    //matchOrientation = ,
                    //requestTime = ,
                };
        
                provider.QueueTeleportRequest(request);
            }
            
            // after all turn ray off
            rayInteractor.enabled = false;
            _isActive = false;
        }
    }

    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = true;
        _isActive = true;
    }

    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = false;
        _isActive = false;
    }
}
