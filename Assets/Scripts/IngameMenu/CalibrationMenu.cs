using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CalibrationMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private GameObject menuUI;
    
    [SerializeField] private SaveWallHandler teleportHandler;
    
    [SerializeField] private List<GameObject> buttons;

    [SerializeField] private UnityEvent onButtonDown;
    [SerializeField] private UnityEvent onButtonUp;


    private InputAction leftHandPrimaryAction;
    private InputAction rightHandPrimaryAction;
    private InputActionMap leftHandLocomotion;
    private InputActionMap rightHandLocomotion;
    private InputActionMap rightHandCalibration;
    
    private void Start()
    {
        leftHandPrimaryAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Primary Action");
        rightHandPrimaryAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Primary Action");
        leftHandLocomotion = actionAsset.FindActionMap("XRI LeftHand Locomotion");
        rightHandLocomotion = actionAsset.FindActionMap("XRI RightHand Locomotion");

        leftHandPrimaryAction.Disable();
        rightHandPrimaryAction.Disable();
        leftHandLocomotion.Disable();
        rightHandLocomotion.Disable();
        
        rightHandCalibration = actionAsset.FindActionMap("XRI RightHand Calibration");
        rightHandCalibration.FindAction("Calibrate").performed += ActivateMenu;
    }
    
    private void OnEnable()
    {
        CloseToUICollision.OnEnterUIArea += Enter;
        CloseToUICollision.OnExitUIArea += Exit;
    }

    private void OnDisable()
    {
        CloseToUICollision.OnEnterUIArea -= Enter;
        CloseToUICollision.OnExitUIArea -= Exit;
    }

    private void Enter(GameObject triggered)
    {
        foreach (GameObject button in buttons)
        {
            if (button.Equals(triggered))
            {
                onButtonDown.Invoke();
                button.GetComponent<ButtonVis>().OnDown();
            }
        }
    }

    private void Exit(GameObject triggered)
    {
        foreach (GameObject button in buttons)
        {
            if (button.Equals(triggered))
            {
                onButtonUp.Invoke();
                button.GetComponent<ButtonVis>().OnUp();
                button.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    private void ActivateMenu(InputAction.CallbackContext obj)
    {
        menuUI.SetActive(true);
    }

    public void ConfirmCalibration()
    {
        leftHandPrimaryAction.Enable();
        rightHandPrimaryAction.Enable();
        leftHandLocomotion.Enable();
        rightHandLocomotion.Enable();
        
        rightHandCalibration.Disable();
        
        menuUI.SetActive(false);
        teleportHandler.ConfirmCalibration();
    }
}
