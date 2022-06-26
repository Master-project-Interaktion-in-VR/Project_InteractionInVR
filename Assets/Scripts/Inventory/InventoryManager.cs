using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    //[SerializeField] private GameObject itemInHand;

    [SerializeField] private Inventory inventory;

    private void Start()
    {
        inventory.antennaParts = new List<GameObject>();
    }

    // public void PutItemInHand(GameObject item)
    // {
    //     itemInHand = item;
    // }
    //
    // public bool HandIsFree()
    // {
    //     return itemInHand == null;
    // }

    public void PutItemInInventory(ActivateEventArgs args)
    {
        var item = args.interactableObject.transform.gameObject;
        inventory.antennaParts.Add(item);
        item.SetActive(false);
    }
}
