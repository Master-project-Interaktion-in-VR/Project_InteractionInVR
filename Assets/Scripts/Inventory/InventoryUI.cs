using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;

    void Start()
    {
        DisplayWristUI();
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisplayWristUI();
        }
    }

    public void PressButton()
    {
        Debug.Log("dein vataaa");
    }

    public void DisplayWristUI()
    {
        if (activeWristUI)
        {
            wristUI.SetActive(false);
            activeWristUI = false;
        }
        else if (!activeWristUI)
        {
            wristUI.SetActive(true);
            activeWristUI = true;
        }
    }

    void Update()
    {
        
    }
}