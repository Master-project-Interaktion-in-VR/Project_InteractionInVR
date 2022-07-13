using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameMenuController : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference menuTriggerActionReference;

    [SerializeField]
    private GameObject holoMenu;

    private bool _isMenuVisible;

    private void OnEnable()
    {
        menuTriggerActionReference.action.performed += OnMenuToggle;
    }
    private void OnDisable()
    {
        menuTriggerActionReference.action.performed -= OnMenuToggle;
    }

    private void OnMenuToggle(InputAction.CallbackContext callback)
    {
        holoMenu.SetActive(!_isMenuVisible);
        _isMenuVisible = !_isMenuVisible;
    }

    public void OnClickedContinueButton()
    {
        holoMenu.SetActive(!_isMenuVisible);
        _isMenuVisible = !_isMenuVisible;
    }

    public void OnClickedMenuButton()
    {

    }

    public void OnClickedQuitButton()
    {
        Application.Quit();
    }
}
