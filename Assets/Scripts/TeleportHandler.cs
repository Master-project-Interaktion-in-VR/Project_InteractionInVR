using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Teleport;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportHandler : MonoBehaviour
{
    public static GameObject saveWall;
    public static bool teleported = false;

    void Start()
    {
        saveWall = this.gameObject;
        GameObject.Find("Environment/Terrain/PlayArea/PaintTerrain").GetComponent<TeleportationArea>().teleporting.AddListener(ActivateTeleport);
        saveWall.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SaveWallTrigger" && teleported)
        {
            Debug.Log("--- Collided with saveWall! " + other.gameObject.name);
            saveWall.GetComponent<MeshRenderer>().enabled = true;
            saveWall.transform.Find("Table_Surface_SaveWall").GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(DisableWall());
        }
    }

    public void ActivateTeleport(TeleportingEventArgs args)
    {
        Debug.Log("Teleport Started");
        teleported = true;
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