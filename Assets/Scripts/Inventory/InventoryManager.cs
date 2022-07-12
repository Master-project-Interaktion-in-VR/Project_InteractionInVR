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
    }

    private void Update()
    {
        if (inventory.activeInHierarchy)
        {
            inventory.transform.position = leftAnchor.transform.position;
            var eulerAngles = leftAnchor.transform.eulerAngles;
            inventory.transform.eulerAngles = new Vector3(eulerAngles.x + 15, eulerAngles.y, 0);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OpenCloseLeftInventory(new InputAction.CallbackContext());
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

    public void PutStickInHand()
     {
         for (int i = 0; i < inventoryItems.Count; i++)
         {
             if (inventoryItems[i].name == "Magnifying Glass")
             {
                 Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                 Destroy(GameObject.Find("MetalDetector(Clone)"));
                 rightHandModel.GetComponent<MeshRenderer>().enabled = false;
                 stickButton.interactable = false;
                 detectorButton.interactable = true;
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

    public void PutDetectorInHand()
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
                //stickButton.gameObject.SetActive(false);
            }
        }
    }
}
