using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject leftAnchor;
    [SerializeField] private GameObject rightAnchor;
    
    [SerializeField] private InputActionReference leftUIActivationReference;
    [SerializeField] private InputActionReference rightUIActivationReference;
    [SerializeField] private InputActionReference takeStickfromInventoryReference;
    [SerializeField] private InputActionReference takeDetectorfromInventoryReference;

    [SerializeField] private int antennaPartsPickedUp;
    [SerializeField] private int maxAntennaParts;

    public GameObject rightHandModel;

    public Transform handSpawn;

    public List<GameObject> inventoryItems = new List<GameObject>();

    public Button stickButton;
    public Button detectorButton;

    private bool _isRight;

    private void Start()
    {
        leftUIActivationReference.action.performed += OpenCloseLeftInventory;
        rightUIActivationReference.action.performed += OpenCloseRightInventory;
        takeStickfromInventoryReference.action.performed += PutStickInHand;
        takeDetectorfromInventoryReference.action.performed += PutDetectorInHand;
    }

    private void Update()
    {
        if (inventory.activeInHierarchy)
        {
            inventory.transform.position = leftAnchor.transform.position;
            var eulerAngles = leftAnchor.transform.eulerAngles;
            inventory.transform.eulerAngles = new Vector3(eulerAngles.x + 15, eulerAngles.y, 0);
        }

        
       /* if (Input.GetKeyDown(KeyCode.DownArrow))  FOR DEVICE SIMULATOR
        {
            OpenCloseLeftInventory(new InputAction.CallbackContext());
        }*/

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
    
    private void OpenCloseLeftInventory(InputAction.CallbackContext obj)
    {
        inventory.SetActive(!inventory.activeInHierarchy);
    }
    
    private void OpenCloseRightInventory(InputAction.CallbackContext obj)
    {
        inventory.SetActive(!inventory.activeInHierarchy);
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
                //stickButton.gameObject.SetActive(false);
            }
             /*else if (inventoryItems[i].name == "MetalDetector")
             {
                 Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                 detectorButton.interactable = false;
                 stickButton.interactable = true;
            }*/
         }
     }

    public void PutDetectorInHand(InputAction.CallbackContext obj)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].name == "Metal Detector")
            {
                Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                Destroy(GameObject.Find("Magnifying Glass(Clone)"));
                rightHandModel.GetComponent<MeshRenderer>().enabled = false;
                stickButton.interactable = true;
                detectorButton.interactable = false;
                inventory.SetActive(false);
                //stickButton.gameObject.SetActive(false);
            }
        }
    }
}
