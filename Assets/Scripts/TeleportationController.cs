using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TeleportationController : MonoBehaviour
{
    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject teleportationRay;

    [SerializeField] private InputActionReference teleportActivationReference;

    [Space]
    [SerializeField] private UnityEvent onTeleportActivate;
    [SerializeField] private UnityEvent onTeleportCancel;

    private void Start()
    {
        teleportActivationReference.action.performed += ActivateTeleport;
        teleportActivationReference.action.canceled += CancelTeleport;
    }

    private void ActivateTeleport(InputAction.CallbackContext obj) => onTeleportActivate.Invoke();

    private void CancelTeleport(InputAction.CallbackContext obj) => Invoke(nameof(DeactivateTeleport), .1f);

    private void DeactivateTeleport() => onTeleportCancel.Invoke();
}
