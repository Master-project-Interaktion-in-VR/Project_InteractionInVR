using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenuController : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference menuTriggerActionReference;

    [SerializeField]
    private GameObject holoMenu;

    [SerializeField]
    private GameObject pauseMenuContainer;

    [SerializeField]
    private GameObject confirmMenuContainer;

    [SerializeField]
    private List<GameObject> buttons;

    [SerializeField]
    private UnityEvent onButtonDown;

    [SerializeField]
    private UnityEvent onButtonUp;


    private bool _isMenuVisible;
    private bool _quitting;


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

    private void OnMenuToggle(InputAction.CallbackContext callback)
    {
        onButtonUp.Invoke();
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
        pauseMenuContainer.SetActive(false);
        confirmMenuContainer.SetActive(true);
    }

    public void OnClickedQuitButton()
    {
        _quitting = true;
        pauseMenuContainer.SetActive(false);
        confirmMenuContainer.SetActive(true);
    }

    public void OnConfirm()
    {
        if (_quitting)
        {
            Application.Quit();
        }
        else
        {
            // to menu
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
                PhotonNetwork.Disconnect();
            }
            SceneSpanningData.isComingFromGame = true;
            SceneManager.LoadScene(GUIConstants.MENU_SCENE);
        }
    }

    public void OnAbort()
    {
        _quitting = false;
        pauseMenuContainer.SetActive(true);
        confirmMenuContainer.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
