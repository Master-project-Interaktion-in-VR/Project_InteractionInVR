using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CalibrationMenu : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset actionAsset;
    
    [SerializeField] 
    private GameObject menuUI;
    
    [SerializeField] 
    private TeleportHandler teleportHandler;
    
    [SerializeField] 
    private List<GameObject> buttons;

    [SerializeField] 
    private UnityEvent onButtonDown;
    
    [SerializeField] 
    private UnityEvent onButtonUp;


    private InputAction _leftHandPrimaryAction;
    private InputAction _rightHandPrimaryAction;
    private InputActionMap _leftHandLocomotion;
    private InputActionMap _rightHandLocomotion;
    private InputActionMap _rightHandCalibration;
    
    private void Start()
    {
        // Get Primary Action and all Locomotion Actions for both Controllers
        _leftHandPrimaryAction = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Primary Action");
        _rightHandPrimaryAction = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Primary Action");
        _leftHandLocomotion = actionAsset.FindActionMap("XRI LeftHand Locomotion");
        _rightHandLocomotion = actionAsset.FindActionMap("XRI RightHand Locomotion");

        // Disable all from the beginning -> you can't move or open inventory before calibrating
        _leftHandPrimaryAction.Disable();
        _rightHandPrimaryAction.Disable();
        _leftHandLocomotion.Disable();
        _rightHandLocomotion.Disable();
        
        // Action for right hand calibration (primary button on right controller)
        _rightHandCalibration = actionAsset.FindActionMap("XRI RightHand Calibration");
        _rightHandCalibration.FindAction("Calibrate").performed += ActivateMenu;
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
    
    /// <summary>
    /// After the UI button to confirm the calibration is pressed, all Locomotion and Inventory Actions are enabled
    /// </summary>
    public void ConfirmCalibration()
    {
        _leftHandPrimaryAction.Enable();
        _rightHandPrimaryAction.Enable();
        _leftHandLocomotion.Enable();
        _rightHandLocomotion.Enable();
        
        _rightHandCalibration.Disable(); // disable calibration action
        
        menuUI.SetActive(false); // disable calibration menu
        teleportHandler.ConfirmCalibration(); // invoke teleportHandler method to enable the save wall
    }

    /// <summary>
    /// Called when hands collide with UI button
    /// Checks which button is pressed and invokes methods to play sound and do some visualisation
    /// </summary>
    /// <param name="triggered"> button which is pressed/collided with </param>
    private void Enter(GameObject triggered)
    {
        foreach (var button in buttons.Where(button => button.Equals(triggered)))
        {
            onButtonDown.Invoke();
            button.GetComponent<ButtonVis>().OnDown();
        }
    }

    /// <summary>
    /// Called when hands exits UI button
    /// Checks which button is exited and invokes methods to play sound, do some visualisation and confirm calibration
    /// </summary>
    /// <param name="triggered"> button which is exited </param>
    private void Exit(GameObject triggered)
    {
        foreach (var button in buttons.Where(button => button.Equals(triggered)))
        {
            onButtonUp.Invoke();
            button.GetComponent<ButtonVis>().OnUp();
            button.GetComponent<Button>().onClick.Invoke();
        }
    }

    /// <summary>
    /// Enable calibration menu when first time calibrated by pressing the right primary button
    /// </summary>
    /// <param name="obj"></param>
    private void ActivateMenu(InputAction.CallbackContext obj)
    {
        menuUI.SetActive(true);
    }
}
