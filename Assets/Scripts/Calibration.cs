using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    Transform CameraRig;
    Transform handMarker;  //the controller on the hand
    Transform fixedMarker; //the fixed controller

    void Start()
    {
        CameraRig = GameObject.Find("MRTK-Quest_OVRCameraRig(Clone)").transform;
        handMarker = CameraRig.FindChildRecursive("RightControllerAnchor").transform;
        fixedMarker = this.transform;
    }

    void Update()
    {
        if (OVRInput.GetActiveController() == OVRInput.Controller.Touch)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) //detect is button 'A' has been pressed
            {
                Vector3 posOffset = fixedMarker.position - handMarker.position; //calculate the difference in positions
                CameraRig.position += posOffset; //offset the position of the cameraRig to realign the controllers

                Vector3 rotOffset = fixedMarker.eulerAngles - handMarker.eulerAngles; //calculate the difference in rotations
                CameraRig.RotateAround(handMarker.position, Vector3.up, rotOffset.y); //using the hand as a pivot, rotate around Y
            }
        }
    }
}
