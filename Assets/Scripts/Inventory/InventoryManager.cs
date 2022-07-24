using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using Photon.Pun;
using TMPro;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject leftArrows;
    [SerializeField] private GameObject rightArrows;
    [SerializeField] private GameObject detector;
    [SerializeField] private GameObject glass;
    [SerializeField] private TMP_Text antennaPartCounter;
    [SerializeField] private List<Image> antennaPartsUI;
        
    [SerializeField] private Inventory inventory;

    [SerializeField] private Animator detectorAnimator;
    [SerializeField] private Animator glassAnimator;

    [SerializeField] private InputActionAsset actionAsset;
    
    [SerializeField] private Transform itemAnchor;

    [SerializeField] private int antennaPartsPickedUp;
    [SerializeField] private int maxAntennaParts;

    private bool _isRight;

    private GameObject leftAnchor;
    private GameObject rightAnchor;
    private GameObject itemInLeftHand;
    private GameObject itemInRightHand;
    private GameObject itemObject;

    private ActionBasedSnapTurnProvider snapTurnScript;
    public string GameScene_name;

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

        leftHandLocomotion.FindAction("Turn").performed += SwitchItemLeft;
        rightHandLocomotion.FindAction("Turn").performed += SwitchItemRight;

        leftAnchor = GameObject.FindGameObjectWithTag("Left Inventory Anchor");
        rightAnchor = GameObject.FindGameObjectWithTag("Right Inventory Anchor");
        snapTurnScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionBasedSnapTurnProvider>();

        inventory.collectedAntennaParts = new bool[maxAntennaParts];
    }

    private void Update()
    {
        if (inventoryUI.activeInHierarchy)
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
        inventoryUI.transform.position = anchor.transform.position;
        var eulerAngles = anchor.transform.eulerAngles;
        inventoryUI.transform.eulerAngles = new Vector3(eulerAngles.x + 15, eulerAngles.y, 0);
    }

    public IEnumerator PutItemInInventory(GameObject item)
    {
        item.GetComponent<Animator>().SetBool("shrink", true); // currently not as RPC
        antennaPartsPickedUp++;
        yield return new WaitForSeconds(0.9f);
        UpdateAntennaPartsUI();
        item.GetPhotonView().RequestOwnership(); // just to be sure
        PhotonNetwork.Destroy(item);

        if (item == itemInLeftHand)
            itemInLeftHand = null;
        else if (item == itemInRightHand)
            itemInRightHand = null;

        if (antennaPartsPickedUp == maxAntennaParts)
            NextLevel();
    }

    public void PutItemInLeftHand(SelectEnterEventArgs args)
    {
        itemInLeftHand = args.interactableObject.transform.gameObject;
        //itemInLeftHand.GetComponent<Rigidbody>().useGravity = true;

        // if an object is grabbed
        if (inventoryUI.activeInHierarchy && itemInLeftHand != null)
            ResetInventoryProperties();
    }

    public void PutItemInRightHand(SelectEnterEventArgs args)
    {
        itemInRightHand = args.interactableObject.transform.gameObject;
       //itemInRightHand.GetComponent<Rigidbody>().useGravity = true;

        // if an object is grabbed
        if (inventoryUI.activeInHierarchy && itemInRightHand != null)
            ResetInventoryProperties();
    }

    public void DropItemFromLeftHand(SelectExitEventArgs args)
    {
        //itemInLeftHand.GetComponent<Rigidbody>().useGravity = true;
        itemInLeftHand = null;
    }

    public void DropItemFromRightHand(SelectExitEventArgs args)
    {
        //itemInRightHand.GetComponent<Rigidbody>().useGravity = true;
        itemInRightHand = null;
    }
    
    private void SwitchItemLeft(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector2>();

        if (!_isRight)
            SwitchItem(dir);
    }

    private void SwitchItemRight(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector2>();

        if (_isRight)
            SwitchItem(dir);
    }

    private void SwitchItem(Vector2 dir)
    {
        if (inventoryUI.activeInHierarchy && dir.x != .5f)
        {
            var right = dir.x > .5f;
            rightArrows.SetActive(right);
            leftArrows.SetActive(!right);
            detectorAnimator.SetBool("scale", !right);
            glassAnimator.SetBool("scale", right);
        }
    }

    // For Device Simulator
    private void SwitchItem(bool right)
    {
        if (inventoryUI.activeInHierarchy)
        {
            rightArrows.SetActive(right);
            leftArrows.SetActive(!right);
            detectorAnimator.SetBool("scale", !right);
            glassAnimator.SetBool("scale", right);
        }
    }

    private void OpenCloseRightInventory(InputAction.CallbackContext obj)
    {
        if (inventoryUI.activeInHierarchy && _isRight || !inventoryUI.activeInHierarchy && itemInRightHand == null)
        {
            _isRight = true;
            SetInventoryAnchor(rightAnchor);
            ResetInventoryProperties();
        }
    }

    private void OpenCloseLeftInventory(InputAction.CallbackContext obj)
    {
        if (inventoryUI.activeInHierarchy && !_isRight || !inventoryUI.activeInHierarchy && itemInLeftHand == null)
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
        inventoryUI.SetActive(!inventoryUI.activeInHierarchy);
        detectorAnimator.SetBool("scale", true);
        glassAnimator.SetBool("scale", false);
        snapTurnScript.enabled = !inventoryUI.activeInHierarchy;
    }

    private void UpdateAntennaPartsUI()
    {
        for (int i = 0; i < antennaPartsUI.Count; i++)
        {
            if (inventory.collectedAntennaParts[i])
            {
                antennaPartsUI[i].color = Color.white;
            }
        }

        antennaPartCounter.text = antennaPartsPickedUp + " / " + maxAntennaParts;
    }

    private void NextLevel()
    {
        PhotonNetwork.LoadLevel(GameScene_name);
    }

    public void SpawnItem(InputAction.CallbackContext obj)
    {
        if (inventoryUI.activeInHierarchy)
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

            var anchorPos = new Vector2(itemAnchor.position.x, itemAnchor.position.z);
            var prefabPos = prefab.transform.position;
            anchorPos += new Vector2(itemAnchor.forward.x, itemAnchor.forward.z) * prefabPos.z;
            var spawnPosition = new Vector3(anchorPos.x, itemAnchor.position.y - prefabPos.y, anchorPos.y);
            
            Quaternion spawnRotation;
            if(prefab.CompareTag("Detector"))
                spawnRotation = Quaternion.Euler(0, itemAnchor.eulerAngles.y, 0);
            else
                spawnRotation = Quaternion.Euler(90, 0, itemAnchor.eulerAngles.y * -1);

            itemObject = PhotonNetwork.Instantiate("Items/" + prefab.name, spawnPosition, spawnRotation);
            var itemRb = itemObject.GetComponent<Rigidbody>();
            itemRb.velocity = Vector3.zero;
            inventoryUI.SetActive(false);
            snapTurnScript.enabled = true;
        }
    }

    public bool DetectorIsInRightHand()
    {
        return itemInRightHand != null && itemInRightHand.CompareTag("Detector");
    }
    
    public bool DetectorIsInLeftHand()
    {
        return itemInLeftHand != null && itemInLeftHand.CompareTag("Detector");
    }
}
