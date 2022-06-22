using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    Transform CameraRig;
    Transform handMarker;  //the controller on the hand
    Transform fixedMarker; //the fixed controller
    public GameObject table_Prefab;
    public static GameObject table;

    void Start()
    {
        CameraRig = GameObject.Find("MRTK-Quest_OVRCameraRig(Clone)").transform;
        handMarker = CameraRig.FindChildRecursive("RightControllerAnchor").transform;
    }

    void Update()
    {
        if (OVRInput.GetActiveController() == OVRInput.Controller.Touch)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) //detect is button 'A' has been pressed
            {
                if(table == null)
                {
                    table = Instantiate(table_Prefab);
                }

                fixedMarker = GameObject.Find("fixedMarker").transform;
                Vector3 posOffset = fixedMarker.position - handMarker.position; //calculate the difference in positions
                //CameraRig.transform.position += posOffset; //offset the position of the cameraRig to realign the controllers

                fixedMarker.transform.parent = null;
                table.transform.parent = fixedMarker.transform;

                fixedMarker.transform.position = handMarker.position;

                Vector3 rotOffset = fixedMarker.eulerAngles - handMarker.eulerAngles; //calculate the difference in rotations
                fixedMarker.transform.rotation = handMarker.rotation;
                //CameraRig.transform.RotateAround(handMarker.position, Vector3.up, rotOffset.y); //using the hand as a pivot, rotate around Y

                table.transform.parent = null;
                fixedMarker.transform.parent = table.transform.parent;
            }
        }
    }
}
