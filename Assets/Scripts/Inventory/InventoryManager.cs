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
    [SerializeField] 
    private GameObject inventoryUI;

    [SerializeField] 
    private GameObject leftArrows;

    [SerializeField] 
    private GameObject rightArrows;

    [SerializeField] 
    private GameObject detector;

    [SerializeField] 
    private GameObject glass;


    [SerializeField] 
    private TMP_Text antennaPartCounter;

    [SerializeField] 
    private List<Image> antennaPartsUI;
        
    [SerializeField] 
    private Inventory inventory;
    
    [SerializeField] 
    private FadeScreen fadeScreen;

    [SerializeField] 
    private Animator detectorAnimator;

    [SerializeField] 
    private Animator glassAnimator;

    [SerializeField] 
    private InputActionAsset actionAsset;

    [SerializeField] 
    private Transform itemAnchor;
    

    [SerializeField] 
    private int antennaPartsPickedUp;

    [SerializeField] 
    private int maxAntennaParts;
    
    
    private static readonly int Scale = Animator.StringToHash("scale");
    private static readonly int Shrink = Animator.StringToHash("shrink");
    
    private bool _isRight;

    private GameObject _leftAnchor;
    private GameObject _rightAnchor;
    private GameObject _itemInLeftHand;
    private GameObject _itemInRightHand;
    private GameObject _itemObject;

    private ActionBasedSnapTurnProvider _snapTurnScript;

    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        
        // Get All Actions for Interaction and Locomotion for both Controllers
        var leftHandAction = actionAsset.FindActionMap("XRI LeftHand Interaction");
        var rightHandAction = actionAsset.FindActionMap("XRI RightHand Interaction");
        var leftHandLocomotion = actionAsset.FindActionMap("XRI LeftHand Locomotion");
        var rightHandLocomotion = actionAsset.FindActionMap("XRI RightHand Locomotion");
        
        // Primary button to open and close inventory
        leftHandAction.FindAction("Primary Action").performed += OpenCloseLeftInventory;
        rightHandAction.FindAction("Primary Action").performed += OpenCloseRightInventory;
        
        // Secondary button to spawn an item from inventory
        leftHandAction.FindAction("Secondary Action").performed += SpawnItem;
        rightHandAction.FindAction("Secondary Action").performed += SpawnItem;
        
        // Joystick to switch between items in inventory
        leftHandLocomotion.FindAction("Turn").performed += SwitchItemLeft;
        rightHandLocomotion.FindAction("Turn").performed += SwitchItemRight;
        
        // Anchor transforms to set inventory an left or right hand
        _leftAnchor = GameObject.FindGameObjectWithTag("Left Inventory Anchor");
        _rightAnchor = GameObject.FindGameObjectWithTag("Right Inventory Anchor");
        
        // Get snapTurnProvider from player to disable while the inventory is open
        var player = GameObject.FindGameObjectWithTag("Player");
        _snapTurnScript = player.GetComponent<ActionBasedSnapTurnProvider>();
        
        // Scriptable object inventory -> resets the Array of collected antenna parts
        inventory.collectedAntennaParts = new bool[maxAntennaParts];
    }

    private void Update()
    {
        // if inventory is active set continuously his position to one of the hand anchors
        if (inventoryUI.activeInHierarchy)
        {
            SetInventoryAnchor(_isRight ? _rightAnchor : _leftAnchor);
        }

        // FOR DEVICE SIMULATOR AND TESTING WITH KEYBOARD
        if (Input.GetKeyDown(KeyCode.UpArrow))
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

    /// <summary>
    /// Sets the inventory transform to the hands anchors and faces towards the player
    /// </summary>
    /// <param name="anchor">anchor on left or right hand where the inventory has to stay</param>
    private void SetInventoryAnchor(GameObject anchor)
    {
        inventoryUI.transform.position = anchor.transform.position;
        var eulerAngles = anchor.transform.eulerAngles;
        inventoryUI.transform.eulerAngles = new Vector3(eulerAngles.x + 15, eulerAngles.y, 0);
    }

    /// <summary>
    /// If an antenna part is collected and the primary button pressed,
    /// this method will store the part in inventory and destroys them after a little shrink animation
    /// </summary>
    /// <param name="item">The antenna part which is collected</param>
    /// <returns>IEnumerator for Coroutine</returns>
    public IEnumerator PutItemInInventory(GameObject item)
    {
        item.GetComponent<Animator>().SetBool(Shrink, true);
        antennaPartsPickedUp++;
        yield return new WaitForSeconds(0.9f);
        UpdateAntennaPartsUI();
        item.GetPhotonView().RequestOwnership(); // just to be sure
        PhotonNetwork.Destroy(item);

        if (item == _itemInLeftHand)
            _itemInLeftHand = null;
        else if (item == _itemInRightHand)
            _itemInRightHand = null;

        // If all antenna parts are picked up, then load the next level
        if (antennaPartsPickedUp == maxAntennaParts)
            StartCoroutine(NextLevel());
    }

    /// <summary>
    /// Called when left hand select triggered (Trigger Button pressed)
    /// </summary>
    /// <param name="args"> Event args to get items which is grabbed </param>
    public void PutItemInLeftHand(SelectEnterEventArgs args)
    {
        _itemInLeftHand = args.interactableObject.transform.gameObject;

        // if an object is grabbed
        if (inventoryUI.activeInHierarchy && _itemInLeftHand != null)
            ResetInventoryProperties();
    }

    /// <summary>
    /// Called when right hand select triggered (Trigger Button pressed)
    /// </summary>
    /// <param name="args"> Event args to get items which is grabbed </param>
    public void PutItemInRightHand(SelectEnterEventArgs args)
    {
        _itemInRightHand = args.interactableObject.transform.gameObject;

        // if an object is grabbed
        if (inventoryUI.activeInHierarchy && _itemInRightHand != null)
            ResetInventoryProperties();
    }

    /// <summary>
    /// Called when left hand select exited (Trigger Button released)
    /// </summary>
    /// <param name="args"> Event args to get items which is released </param>
    public void DropItemFromLeftHand(SelectExitEventArgs args)
    {
        var transformName = args.interactableObject.transform.name;
        
        // If item is detector or magnifying glass, enable their gravity
        if (transformName.Contains(detector.name) || transformName.Contains(glass.name))
            _itemInLeftHand.GetComponent<NetworkHelper>().SetGravity(true);
        
        _itemInLeftHand = null;
    }

    /// <summary>
    /// Called when right hand select exited (Trigger Button released)
    /// </summary>
    /// <param name="args"> Event args to get items which is released </param>
    public void DropItemFromRightHand(SelectExitEventArgs args)
    {
        var transformName = args.interactableObject.transform.name;
        
        // If item is detector or magnifying glass, enable their gravity
        if (transformName.Contains(detector.name) || transformName.Contains(glass.name))
            _itemInRightHand.GetComponent<NetworkHelper>().SetGravity(true);
        
        _itemInRightHand = null;
    }
    
    /// <summary>
    /// Load next level with photon
    /// </summary>
    [PunRPC]
    public void NextLevelRpc()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    /// <summary>
    /// Public method to check if detector is in right hand to send vibration signals from pc client
    /// </summary>
    /// <returns> true when detector is in right hand, false if not </returns>
    public bool DetectorIsInRightHand()
    {
        return _itemInRightHand != null && _itemInRightHand.CompareTag("Detector");
    }
    
    /// <summary>
    /// Public method to check if detector is in left hand to send vibration signals from pc client
    /// </summary>
    /// <returns> true when detector is in left hand, false if not </returns>
    public bool DetectorIsInLeftHand()
    {
        return _itemInLeftHand != null && _itemInLeftHand.CompareTag("Detector");
    }
    
    /// <summary>
    /// Called when left joystick is moved to switch items in left inventory
    /// </summary>
    /// <param name="obj"> Context from InputAction to read which direction its moved </param>
    private void SwitchItemLeft(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector2>();

        if (!_isRight)
            SwitchItem(dir);
    }

    /// <summary>
    /// Called when right joystick is moved to switch items in right inventory
    /// </summary>
    /// <param name="obj"> Context from InputAction to read which direction its moved </param>
    private void SwitchItemRight(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector2>();

        if (_isRight)
            SwitchItem(dir);
    }

    /// <summary>
    /// Highlights an item in inventory depend on the direction
    /// </summary>
    /// <param name="dir"> the direction where the thumbstick is moved </param>
    private void SwitchItem(Vector2 dir)
    {
        if (inventoryUI.activeInHierarchy && dir.x != 0.5f)
        {
            var right = dir.x > .5f;
            rightArrows.SetActive(right);
            leftArrows.SetActive(!right);
            detectorAnimator.SetBool(Scale, !right);
            glassAnimator.SetBool(Scale, right);
        }
    }

    // Switch item for Device Simulator to test with Keyboard
    private void SwitchItem(bool right)
    {
        if (inventoryUI.activeInHierarchy)
        {
            rightArrows.SetActive(right);
            leftArrows.SetActive(!right);
            detectorAnimator.SetBool(Scale, !right);
            glassAnimator.SetBool(Scale, right);
        }
    }

    /// <summary>
    /// Called when primary button on right hand is pressed.
    /// Opens and closes right inventory by enabling its UI under special conditions
    /// </summary>
    /// <param name="obj"></param>
    private void OpenCloseRightInventory(InputAction.CallbackContext obj)
    {
        // Either the inventory is already active on the right hand or not active and there is no item in right hand
        // --> you can only close the right inventory with the right primary button
        // --> you can only open the right inventory if there is nothing on the right hand
        if (inventoryUI.activeInHierarchy && _isRight || !inventoryUI.activeInHierarchy && _itemInRightHand == null)
        {
            _isRight = true;
            SetInventoryAnchor(_rightAnchor);
            ResetInventoryProperties();
        }
    }

    /// <summary>
    /// Called when primary button on left hand is pressed.
    /// Opens and closes left inventory by enabling its UI under special conditions
    /// </summary>
    /// <param name="obj"></param>
    private void OpenCloseLeftInventory(InputAction.CallbackContext obj)
    {
        // Either the inventory is already active on the left hand or not active and there is no item in left hand
        // --> you can only close the left inventory with the left primary button
        // --> you can only open the left inventory if there is nothing on the left hand
        if (inventoryUI.activeInHierarchy && !_isRight || !inventoryUI.activeInHierarchy && _itemInLeftHand == null)
        {
            _isRight = false;
            SetInventoryAnchor(_leftAnchor);
            ResetInventoryProperties();
        }
    }

    /// <summary>
    /// Resets the inventory properties after its closed
    /// </summary>
    private void ResetInventoryProperties()
    {
        leftArrows.SetActive(true);
        rightArrows.SetActive(false);
        inventoryUI.SetActive(!inventoryUI.activeInHierarchy);
        detectorAnimator.SetBool(Scale, true);
        glassAnimator.SetBool(Scale, false);
        _snapTurnScript.enabled = !inventoryUI.activeInHierarchy;
    }

    /// <summary>
    /// Updates the UI to show which and how much antenna part(s) are already collected
    /// </summary>
    private void UpdateAntennaPartsUI()
    {
        for (var i = 0; i < maxAntennaParts; i++)
        {
            if (inventory.collectedAntennaParts[i])
            {
                antennaPartsUI[i].color = new Color(0, 1, 0.6f); // antenna part color -> light green
            }
        }

        antennaPartCounter.text = antennaPartsPickedUp + " / " + maxAntennaParts;
    }
    
    /// <summary>
    /// Calls the public NextLevelRpc method with photonView after a fadeout animation
    /// </summary>
    /// <returns> IEnumerator for Coroutine </returns>
    private IEnumerator NextLevel()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        
        _photonView.RPC("NextLevelRpc", RpcTarget.All);
    }

    /// <summary>
    /// Called when secondary button is pressed.
    /// Spawns the selected item right in front of the player
    /// </summary>
    /// <param name="obj"></param>
    private void SpawnItem(InputAction.CallbackContext obj)
    {
        if (!inventoryUI.activeInHierarchy) return;
           
        // If there is already a magnifying glass or detector spawned, remove it from hand and destroy it
        if (_itemObject == _itemInLeftHand)
            _itemInLeftHand = null;
        else if (_itemObject == _itemInRightHand)
            _itemInRightHand = null;

        if (_itemObject != null)
            PhotonNetwork.Destroy(_itemObject);

        // Spawn magnifying glass if right item is selected and detector if not (left item is selected)
        var prefab = rightArrows.activeInHierarchy ? glass : detector;

        var position = itemAnchor.position;
        var anchorPos = new Vector2(position.x, position.z);
        var prefabPos = prefab.transform.position;
        var forward = itemAnchor.forward;
        
        anchorPos += new Vector2(forward.x, forward.z) * prefabPos.z;
        
        // Set spawnPosition depending on itemAnchor (VR Headset) position and prefab position (detector should be lower than glass)
        var spawnPosition = new Vector3(anchorPos.x, position.y - prefabPos.y, anchorPos.y);

        // Set rotation depending on where the player is facing
        var eulerAngles = itemAnchor.eulerAngles;
        var spawnRotation = prefab.CompareTag("Detector") ? Quaternion.Euler(0, eulerAngles.y, 0) : Quaternion.Euler(90, 0, eulerAngles.y * -1);

        // Instantiate item with photon
        _itemObject = PhotonNetwork.Instantiate("Items/" + prefab.name, spawnPosition, spawnRotation);
        
        // reset values
        var itemRb = _itemObject.GetComponent<Rigidbody>();
        itemRb.velocity = Vector3.zero;
        inventoryUI.SetActive(false);
        _snapTurnScript.enabled = true;
    }
}
