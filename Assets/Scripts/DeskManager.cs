using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskManager : MonoBehaviour
{
    private GameObject rockTable;
    public void Start()
    {
        rockTable = GameObject.Find("RockTable");
        rockTable.GetComponentInChildren<Renderer>().enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering!");
        if (other.gameObject.name == "PlayerEmpty")
        {
            rockTable.GetComponentInChildren<Renderer>().enabled = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exiting!");
        if (other.gameObject.name == "PlayerEmpty")
        {.cs
            rockTable.GetComponentInChildren<Renderer>().enabled = false;
        }
    }
}
