using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int antennaPartsPickedUp;
    [SerializeField] private int maxAntennaParts;

    public GameObject rightHandModel;

    public Transform handSpawn;

    public List<GameObject> inventoryItems = new List<GameObject>();

    public Button stickButton;
    public Button detectorButton;

    void Start()
    {
       // PutItemInHand();    
    }

    public void PutItemInInventory(ActivateEventArgs args)
    {
        var item = args.interactableObject.transform.gameObject;
        antennaPartsPickedUp++;
        Destroy(item);

        if (antennaPartsPickedUp == maxAntennaParts)
        {
            NextLevel();
        }
    }

    public void DeinVater()
    {
        rightHandModel.GetComponent<MeshRenderer>().enabled = false;
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PutStickInHand()
     {
         for (int i = 0; i < inventoryItems.Count; i++)
         {
             if (inventoryItems[i].name == "Stick")
             {
                 Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                 inventoryItems.Remove(inventoryItems[i]);
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
            if (inventoryItems[i].name == "MetalDetector")
            {
                Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                inventoryItems.Remove(inventoryItems[i]);
                rightHandModel.GetComponent<MeshRenderer>().enabled = false;
                stickButton.interactable = false;
                detectorButton.interactable = true;
                //stickButton.gameObject.SetActive(false);
                GameObject stick = GameObject.Find("Stick");
                inventoryItems.Add(stick);
            }
        }
    }
}
