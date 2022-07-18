using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreCalibration : MonoBehaviour
{
    public GameObject table_prefab;
    GameObject table;

    Transform CameraRig;
    Transform handMarker;  //the controller on the hand
    Transform fixedMarker; //the fixed controller

    // Start is called before the first frame update
    void Start()
    {
        CameraRig = GameObject.Find("MRTK-Quest_OVRCameraRig(Clone)").transform;
        handMarker = CameraRig.FindChildRecursive("RightControllerAnchor").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetActiveController() == OVRInput.Controller.Touch)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) //detect is button 'A' has been pressed
            {
                if (table == null)
                {
                    table = Instantiate(table_prefab);
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

                SceneInformationManager.CrossSceneInformation_position = table.transform.position;
                SceneInformationManager.CrossSceneInformation_rotation = table.transform.rotation;
            }
        }
    }
}
