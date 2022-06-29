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

    /*public void HideHandTest()
    {
        rightHandModel.GetComponent<MeshRenderer>().enabled = false;
    }*/

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
            if (inventoryItems[i].name == "MetalDetector")
            {
                Instantiate(inventoryItems[i], handSpawn.position, handSpawn.rotation);
                Destroy(GameObject.Find("Stick(Clone)"));
                rightHandModel.GetComponent<MeshRenderer>().enabled = false;
                stickButton.interactable = true;
                detectorButton.interactable = false;
                //stickButton.gameObject.SetActive(false);
            }
        }
    }
}
