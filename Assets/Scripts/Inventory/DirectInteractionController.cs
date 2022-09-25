using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectInteractionController : MonoBehaviour
{
    [SerializeField] 
    private bool rightHand;
    
    
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
    }

    /// <summary>
    /// Called when grabbing an item.
    /// Just calls the PutItemInHand method from inventory manager depending on which hand this is
    /// </summary>
    /// <param name="args"></param>
    public void SelectEnter(SelectEnterEventArgs args)
    {
        if (_inventoryManager != null)
        {
            if(rightHand)
                _inventoryManager.PutItemInRightHand(args);
            else
                _inventoryManager.PutItemInLeftHand(args);
        }
    }
    
    /// <summary>
    /// Called when releasing an item.
    /// Just calls the DropItemFromHand method from inventory manager depending on which hand this is
    /// </summary>
    /// <param name="args"></param>
    public void SelectExit(SelectExitEventArgs args)
    {
        if (_inventoryManager != null)
        {
            if(rightHand)
                _inventoryManager.DropItemFromRightHand(args);
            else
                _inventoryManager.DropItemFromLeftHand(args);
        }
    }
}
