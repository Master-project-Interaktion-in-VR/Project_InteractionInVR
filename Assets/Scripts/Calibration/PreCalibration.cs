using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PreCalibration : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset actionAsset;
    
    [SerializeField] 
    private GameObject inventory;

    [SerializeField]
    private GameObject table;

    //the controller on the hand
    [SerializeField]
    private Transform handMarker;  

    //the fixed controller
    private Transform fixedMarker; 

    private Transform CameraRig;

    private void Start()
    {
        CameraRig = GameObject.Find("XR Origin").transform;
        var rightHandAction = actionAsset.FindActionMap("XRI RightHand Calibration");
        rightHandAction.FindAction("Calibrate").performed += Calibrate;

        // check for headset type and set the fixed marker accordingly
        OVRPlugin.SystemHeadset headset = OVRPlugin.GetSystemHeadsetType();
        if (headset == OVRPlugin.SystemHeadset.Oculus_Link_Quest || headset == OVRPlugin.SystemHeadset.Oculus_Quest)
        {
            fixedMarker = GameObject.Find("fixedMarker_quest1").transform;
            GameObject.Find("fixedMarker_quest2").SetActive(false);
        }
        else
        {
            fixedMarker = GameObject.Find("fixedMarker_quest2").transform;
            GameObject.Find("fixedMarker_quest1").SetActive(false);
        }
    }

    /// <summary>
    /// The function takes the position and rotation of the handMarker and calibrates the table
    /// table
    /// </summary>
    /// <param name="obj">The callback context for the action.</param>
    private void Calibrate(InputAction.CallbackContext obj)
    {
        if (!inventory.activeInHierarchy)
        {
            // make the table a child of the fixed marker
            fixedMarker.transform.parent = null;
            table.transform.parent = fixedMarker.transform;

            // set the position and rotation of the table
            fixedMarker.transform.position = handMarker.position;
            fixedMarker.transform.rotation = handMarker.rotation;

            // make the table a parent of the fixed marker
            table.transform.parent = null;
            fixedMarker.transform.parent = table.transform.parent;

            // set the position and rotation of the table for the calibration int the next scene
            SceneInformationManager.CrossSceneInformation_position = table.transform.position;
            SceneInformationManager.CrossSceneInformation_rotation = table.transform.rotation;
        }
    }
}
