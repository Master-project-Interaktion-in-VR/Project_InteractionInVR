using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Teleport;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// When the player collides with the trigger, the saveWall is enabled and then disabled after a few seconds
/// </summary>
public class SaveWallHandler : MonoBehaviour
{
    [SerializeField]
    private static GameObject saveWall;

    private bool confirmed = false;

    private void Start()
    {
        saveWall = this.gameObject;
        saveWall.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// is called when the user confirms the calibration
    /// The function sets the boolean variable "confirmed" to true, disables the saveWall's mesh
    /// renderer, and sets the saveWall's parent to the camera rig
    /// </summary>
    public void ConfirmCalibration()
    {
        confirmed = true;
        saveWall.GetComponent<MeshRenderer>().enabled = false;
        Transform camRig = GameObject.Find("XR Origin").transform;
        saveWall.transform.parent = camRig;
    }

    /// <summary>
    /// When the player collides with the trigger, the saveWall is enabled and then disabled after a few
    /// seconds
    /// </summary>
    /// <param name="Collider">The collider that is being collided with.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SaveWallTrigger" && confirmed)
        {
            saveWall.GetComponent<MeshRenderer>().enabled = true;
            saveWall.transform.Find("Table_Surface_SaveWall").GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(DisableWall());
        }
    }

   /// <summary>
   /// Wait 3 seconds, then disable the wall and the table surface
   /// </summary>
    private IEnumerator DisableWall()
    {
        yield return new WaitForSeconds(3);
        saveWall.GetComponent<MeshRenderer>().enabled = false;
        saveWall.transform.Find("Table_Surface_SaveWall").GetComponent<MeshRenderer>().enabled = false;
    }
}