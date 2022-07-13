using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject leftAnchor;
    [SerializeField] private GameObject rightAnchor;
    [SerializeField] private GameObject leftArrows;
    [SerializeField] private GameObject rightArrows;

    [SerializeField] private Image detectorImage;

    [SerializeField] private Animator animation;

    [SerializeField] private InputActionAsset actionAsset;

    [SerializeField] private ActionBasedSnapTurnProvider snapTurnScript;

    [SerializeField] private int antennaPartsPickedUp;
    [SerializeField] private int maxAntennaParts;

    public GameObject rightHandModel;

    public Transform handSpawn;

    public List<GameObject> inventoryItems = new List<GameObject>();

    public Button stickButton;
    public Button detectorButton;

    private bool _isRight;
    private bool _isRightSelected;

    private Vector3 _originalScale;
    private Vector3 _scaleTo;

    private void Start()
    {
        //leftUIActivationReference.action.performed += OpenCloseLeftInventory; WITHOUT FINDING IN ACTION MAP
        //rightUIActivationReference.action.performed += OpenCloseRightInventory;
        //takeStickfromInventoryReference.action.performed += PutStickInHand;
        //takeDetectorfromInventoryReference.action.performed += PutDetectorInHand;

        _originalScale = detectorImage.transform.localScale;
        _scaleTo = _originalScale * 10;

        var leftHandAction = actionAsset.FindActionMap("XRI LeftHand Interaction");
        var rightHandAction = actionAsset.FindActionMap("XRI RightHand Interaction");
        var leftHandLocomotion = actionAsset.FindActionMap("XRI LeftHand Locomotion");
        var rightHandLocomotion = actionAsset.FindActionMap("XRI RightHand Locomotion");

        leftHandAction.FindAction("Primary Action").performed += OpenCloseLeftInventory;
        rightHandAction.FindAction("Primary Action").performed += OpenCloseRightInventory;
        leftHandAction.FindAction("Secondary Action").performed += PutStickInHand;
        rightHandAction.FindAction("Secondary Action").performed += PutDetectorInHand;

        leftHandLocomotion.FindAction("Turn").performed += SwitchItem;
        rightHandLocomotion.FindAction("Turn").performed += SwitchItem;
    }

    private void Update()
    {
        if (inventory.activeInHierarchy)
        {
            SetInventoryAnchor(_isRight ? rightAnchor : leftAnchor);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))  //FOR DEVICE SIMULATOR
        {           
            OpenCloseRightInventory(new InputAction.CallbackContext());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {            
            OpenCloseLeftInventory(new InputAction.CallbackContext());
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchItem(new InputAction.CallbackContext());
            detectorImage.transform.DOScale(1.6f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            //animation.Play("ItemPopUp");            
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchItem(new InputAction.CallbackContext());
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            PutDetectorInHand(new InputAction.CallbackContext());
        }
    }

    private GameObject SetInventoryAnchor(GameObject anchor)
    {
        inventory.transform.position = anchor.transform.position;
        var eulerAngles = anchor.transform.eulerAngles;
        inventory.transform.eulerAngles = new Vector3(eulerAngles.x + 15, eulerAngles.y, 0);
        return anchor;
    }

    private void SwitchItem(InputAction.CallbackContext obj)
    {
        _isRightSelected = _isRight;

        if (_isRightSelected)
        {
            rightArrows.SetActive(true);
            leftArrows.SetActive(false);
            //animation.Play("ItemPopUp");
        }
        else
        {
            leftArrows.SetActive(true);
            rightArrows.SetActive(false);
            //animation.Play("ItemPopUp");
        }
    }

    public void PutItemInInventory(ActivateEventArgs args)
    {
        var item = args.interactableObject.transform.gameObject;
        item.GetComponent<Animator>().SetBool("shrink", true);
        antennaPartsPickedUp++;
        
        Destroy(item, .9f);
        
        if (antennaPartsPickedUp == maxAntennaParts)
        {
            NextLevel();
        }
    }

    private void OpenCloseRightInventory(InputAction.CallbackContext obj)
    {
        if (inventory.activeInHierarchy && _isRight || !inventory.activeInHierarchy)
        {
            _isRight = true;
            SetInventoryAnchor(rightAnchor);
            leftArrows.SetActive(false);
            rightArrows.SetActive(false);
            inventory.SetActive(!inventory.activeInHierarchy);
            snapTurnScript.enabled = !inventory.activeInHierarchy;
        }
    }

    private void OpenCloseLeftInventory(InputAction.CallbackContext obj)
    {
        if (inventory.activeInHierarchy && !_isRight || !inventory.activeInHierarchy)
        {
            _isRight = false;
            SetInventoryAnchor(leftAnchor);
            leftArrows.SetActive(false);
            rightArrows.SetActive(false);
            inventory.SetActive(!inventory.activeInHierarchy);
            snapTurnScript.enabled = !inventory.activeInHierarchy;
        }  
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PutStickInHand(InputAction.CallbackContext obj)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].name == "Magnifying Glass" && inventory.activeInHierarchy)
            {
                Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                Destroy(GameObject.Find("Metal Detector(Clone)"));
                rightHandModel.GetComponent<MeshRenderer>().enabled = false;
                stickButton.interactable = false;
                detectorButton.interactable = true;
                inventory.SetActive(false);
            }
        }
    }

    public void PutDetectorInHand(InputAction.CallbackContext obj)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].name == "Metal Detector" && inventory.activeInHierarchy && leftArrows.activeInHierarchy)
            {
               // var detectorObject = Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation); USE THIS LATER FOR CLEANER CODE
                Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                Destroy(GameObject.Find("Magnifying Glass(Clone)"));
                rightHandModel.GetComponent<MeshRenderer>().enabled = false;
                stickButton.interactable = true;
                detectorButton.interactable = false;
                inventory.SetActive(false);
            }
        }
    }

    private void PutItemInHand(InputAction.CallbackContext obj)
    {
        // aufpassen in die richtige hand
        // die oberen hier verfrachten
        // input action bei turn east, west
    }
}
