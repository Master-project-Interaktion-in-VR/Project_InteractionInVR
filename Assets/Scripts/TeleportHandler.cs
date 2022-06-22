using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Teleport;
using UnityEngine;

public class TeleportHandler : MonoBehaviour, IMixedRealityTeleportHandler
{
    GameObject saveWall;

    void Start()
    {
        saveWall = this.gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("--- Collided with saveWall!");
        saveWall.GetComponent<MeshRenderer>().enabled = true;
    }

    public void OnTeleportCanceled(TeleportEventData eventData)
    {
        Debug.Log("Teleport Cancelled");
    }

    public void OnTeleportCompleted(TeleportEventData eventData)
    {
        Debug.Log("Teleport Completed");
    }

    public void OnTeleportRequest(TeleportEventData eventData)
    {
        Debug.Log("Teleport Request");
    }

    public void OnTeleportStarted(TeleportEventData eventData)
    {
        Debug.Log("Teleport Started");
        saveWall.GetComponent<MeshRenderer>().enabled = true;
        Transform ovrCameraRig = GameObject.Find("MRTK-Quest_OVRCameraRig(Clone)").transform;
        saveWall.transform.parent = ovrCameraRig;
        foreach (GameObject holdingObject in BuildManager.holdingObjects_List)
        {
            Destroy(holdingObject);
        }
        Destroy(Calibration.table);
    }

    void OnEnable()
    {
        CoreServices.TeleportSystem.RegisterHandler<IMixedRealityTeleportHandler>(this);
    }

    void OnDisable()
    {
        CoreServices.TeleportSystem.UnregisterHandler<IMixedRealityTeleportHandler>(this);
    }
}