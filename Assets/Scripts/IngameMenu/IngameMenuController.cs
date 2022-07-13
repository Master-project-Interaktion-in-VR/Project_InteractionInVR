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

    [SerializeField]
    private List<GameObject> buttons;

    private bool _isMenuVisible;


    private void OnEnable()
    {
        menuTriggerActionReference.action.performed += OnMenuToggle;
        CloseToUICollision.OnEnterUIArea += Enter;
        CloseToUICollision.OnExitUIArea += Exit;
    }

    private void OnDisable()
    {
        menuTriggerActionReference.action.performed -= OnMenuToggle;
        CloseToUICollision.OnEnterUIArea -= Enter;
        CloseToUICollision.OnExitUIArea -= Exit;
    }

    private void Enter(GameObject triggered)
    {
        foreach (GameObject button in buttons)
        {
            if (button.Equals(triggered))
            {
                Debug.Log(button.transform.parent.name);
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
                Debug.Log(button.transform.parent.name);
                button.GetComponent<ButtonVis>().OnUp();
            }
        }
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
