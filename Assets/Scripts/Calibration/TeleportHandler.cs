using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Teleport;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportHandler : MonoBehaviour
{
    public static GameObject saveWall;
    private bool confirmed = false;

    void Start()
    {
        saveWall = this.gameObject;
        saveWall.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SaveWallTrigger" && confirmed)
        {
            Debug.Log("--- Collided with saveWall! " + other.gameObject.name);
            saveWall.GetComponent<MeshRenderer>().enabled = true;
            saveWall.transform.Find("Table_Surface_SaveWall").GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(DisableWall());
        }
    }

    public void ConfirmCalibration()
    {
        confirmed = true;
        saveWall.GetComponent<MeshRenderer>().enabled = false;
        Transform camRig = GameObject.Find("XR Origin").transform;
        saveWall.transform.parent = camRig;
    }

    IEnumerator DisableWall()
    {
        yield return new WaitForSeconds(3);
        saveWall.GetComponent<MeshRenderer>().enabled = false;
        saveWall.transform.Find("Table_Surface_SaveWall").GetComponent<MeshRenderer>().enabled = false;
    }
}