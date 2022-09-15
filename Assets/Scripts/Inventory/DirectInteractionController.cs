using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectInteractionController : MonoBehaviour
{
    [SerializeField] private bool rightHand;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
    }

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
