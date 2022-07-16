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
    [SerializeField] private GameObject itemAnchor;
    [SerializeField] private GameObject leftAnchor;
    [SerializeField] private GameObject rightAnchor;
    [SerializeField] private GameObject leftArrows;
    [SerializeField] private GameObject rightArrows;
    [SerializeField] private GameObject detector;
    [SerializeField] private GameObject glass;

    [SerializeField] private Image detectorImage;

    [SerializeField] private Animator animator;

    [SerializeField] private InputActionAsset actionAsset;

    [SerializeField] private ActionBasedSnapTurnProvider snapTurnScript;

    [SerializeField] private int antennaPartsPickedUp;
    [SerializeField] private int maxAntennaParts;

    private bool _isRight;

    public GameObject itemInLeftHand;
    public GameObject itemInRightHand;
    private GameObject itemObject;

    private void Start()
    {
        var leftHandAction = actionAsset.FindActionMap("XRI LeftHand Interaction");
        var rightHandAction = actionAsset.FindActionMap("XRI RightHand Interaction");
        var leftHandLocomotion = actionAsset.FindActionMap("XRI LeftHand Locomotion");
        var rightHandLocomotion = actionAsset.FindActionMap("XRI RightHand Locomotion");

        leftHandAction.FindAction("Primary Action").performed += OpenCloseLeftInventory;
        rightHandAction.FindAction("Primary Action").performed += OpenCloseRightInventory;
        leftHandAction.FindAction("Secondary Action").performed += SpawnItem;
        rightHandAction.FindAction("Secondary Action").performed += SpawnItem;

       // leftHandLocomotion.FindAction("Turn").performed += SwitchItem;
       // rightHandLocomotion.FindAction("Turn").performed += SwitchItem;
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
            SwitchItem(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchItem(true);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SpawnItem(new InputAction.CallbackContext());
        }
    }

    private void SetInventoryAnchor(GameObject anchor)
    {
        inventory.transform.position = anchor.transform.position;
        var eulerAngles = anchor.transform.eulerAngles;
        inventory.transform.eulerAngles = new Vector3(eulerAngles.x + 15, eulerAngles.y, 0);
    }

    private void SwitchItem(bool right)
    {
        if (inventory.activeInHierarchy)
        {
            rightArrows.SetActive(right);
            leftArrows.SetActive(!right);
            animator.SetBool("scale", !right);
        }   
    }

    public void PutItemInInventory(ActivateEventArgs args)
    {
        var item = args.interactableObject.transform.gameObject;
        item.GetComponent<Animator>().SetBool("shrink", true);
        antennaPartsPickedUp++;

        if (item == itemInLeftHand)
            itemInLeftHand = null;
        else if (item == itemInRightHand)
            itemInRightHand = null;

        Destroy(item);

        if (antennaPartsPickedUp == maxAntennaParts)
            NextLevel();
    }

    public void PutItemInLeftHand(SelectEnterEventArgs args)
    {
        itemInLeftHand = args.interactableObject.transform.gameObject;

        // if an object is grabbed
        if (inventory.activeInHierarchy && itemInLeftHand != null)
            ResetInventoryProperties();
    }

    public void PutItemInRightHand(SelectEnterEventArgs args)
    {
        itemInRightHand = args.interactableObject.transform.gameObject;

        // if an object is grabbed
        if (inventory.activeInHierarchy && itemInRightHand != null)
            ResetInventoryProperties();
    }

    public void DropItemFromLeftHand(SelectExitEventArgs args)
    {
        itemInLeftHand = null;
    }

    public void DropItemFromRightHand(SelectExitEventArgs args)
    {
        itemInRightHand = null;
    }

    private void OpenCloseRightInventory(InputAction.CallbackContext obj)
    {
        if (inventory.activeInHierarchy && _isRight || !inventory.activeInHierarchy && itemInRightHand == null)
        {
            _isRight = true;
            SetInventoryAnchor(rightAnchor);
            ResetInventoryProperties();
        }
    }

    private void OpenCloseLeftInventory(InputAction.CallbackContext obj)
    {
        if (inventory.activeInHierarchy && !_isRight || !inventory.activeInHierarchy && itemInLeftHand == null)
        {       
            _isRight = false;
            SetInventoryAnchor(leftAnchor);
            ResetInventoryProperties();
        }
    }

    private void ResetInventoryProperties()
    {
        leftArrows.SetActive(true);
        rightArrows.SetActive(false);
        inventory.SetActive(!inventory.activeInHierarchy);
        animator.SetBool("scale", true);
        snapTurnScript.enabled = !inventory.activeInHierarchy;
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SpawnItem(InputAction.CallbackContext obj)
    {
        if (inventory.activeInHierarchy)
        {
            if (itemObject == itemInLeftHand)
                itemInLeftHand = null;
            else if (itemObject == itemInRightHand)
                itemInRightHand = null;

            if (itemObject != null)
                Destroy(itemObject);

            GameObject prefab;

            if (rightArrows.activeInHierarchy)           
                prefab = glass;           
            else
                prefab = detector;

            itemObject = Instantiate(prefab, itemAnchor.transform.position, Quaternion.identity);
            inventory.SetActive(false);
            snapTurnScript.enabled = true;
        }
    }
}
