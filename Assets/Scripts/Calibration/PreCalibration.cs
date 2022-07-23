using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PreCalibration : MonoBehaviour
{
    public GameObject table_prefab;
    GameObject table;

    Transform CameraRig;
    public Transform handMarker;  //the controller on the hand
    Transform fixedMarker; //the fixed controller

    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private GameObject inventory;

    // Start is called before the first frame update
    void Start()
    {
        CameraRig = GameObject.Find("XR Origin").transform;
        var rightHandAction = actionAsset.FindActionMap("XRI RightHand Interaction");
        rightHandAction.FindAction("Secondary Action").performed += Calibrate;
    }

    public void Calibrate(InputAction.CallbackContext obj)
    {
        if (!inventory.activeInHierarchy && !TeleportHandler.teleported)
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
