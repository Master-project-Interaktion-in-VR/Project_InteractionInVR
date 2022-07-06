using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationController : MonoBehaviour
{
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private InteractionLayerMask teleportationLayerMask;
    [SerializeField] private InteractionLayerMask noTeleportationLayerMask;

    [SerializeField] private InputActionReference teleportActivationReference;

    [SerializeField] private float startVelocity;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float increaseStep;
    [SerializeField] private float increaseDelay;

    private bool _deactivated;

    private void Start()
    {
        teleportActivationReference.action.performed += ActivateTeleport;
        teleportActivationReference.action.canceled += CancelTeleport;
    }

    private void ActivateTeleport(InputAction.CallbackContext obj)
    {
        rayInteractor.enabled = true;
        controller.enableInputActions = true;
        _deactivated = false;
        rayInteractor.interactionLayers = noTeleportationLayerMask;
        StartCoroutine(IncreaseVelocity());
    }

    private void CancelTeleport(InputAction.CallbackContext obj)
    {
        _deactivated = true;
        Invoke(nameof(DeactivateTeleport), .1f);
    }

    private void DeactivateTeleport()
    {
        rayInteractor.enabled = false;
        controller.enableInputActions = false;
        rayInteractor.velocity = startVelocity;
    }

    private IEnumerator IncreaseVelocity()
    {
        while (rayInteractor.velocity < maxVelocity && !_deactivated)
        {
            yield return new WaitForSeconds(increaseDelay);
            rayInteractor.velocity += increaseStep;
        }

        if (rayInteractor.velocity >= maxVelocity)
        {
            rayInteractor.interactionLayers = teleportationLayerMask;
        }
    }
}
