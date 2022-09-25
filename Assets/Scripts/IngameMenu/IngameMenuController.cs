using Photon.Pun;
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

    /// <summary>
    /// Invoke the button down event on the corresponding button.
    /// </summary>
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

    /// <summary>
    /// Invoke the button up event on the corresponding button.
    /// </summary>
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

    /// <summary>
    /// Toggle menu visibility.
    /// </summary>
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

    /// <summary>
    /// Handle the confirmation request which is used for quit and menu options.
    /// </summary>
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

    /// <summary>
    /// Confirmation request was aborted.
    /// </summary>
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
