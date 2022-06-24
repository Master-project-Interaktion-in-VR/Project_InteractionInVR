using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemInHand;

    [SerializeField] private List<GameObject> items = new List<GameObject>();

    public void PutItemInHand(GameObject item)
    {
        itemInHand = item;
    }

    public bool HandIsFree()
    {
        return itemInHand == null;
    }

    private void Update()
    {
        
    }
}
