using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PreCalibration : MonoBehaviour
{
    public GameObject table_prefab;
    public GameObject table;

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

    public void Calibrate(InputAction.CallbackContext obj)
    {
        if (!inventory.activeInHierarchy && !TeleportHandler.teleported)
        {
            if (table == null)
            {
                table = Instantiate(table_prefab);
            }

            fixedMarker.transform.parent = null;
            table.transform.parent = fixedMarker.transform;

            fixedMarker.transform.position = handMarker.position;

            fixedMarker.transform.rotation = handMarker.rotation;

            table.transform.parent = null;
            fixedMarker.transform.parent = table.transform.parent;

            SceneInformationManager.CrossSceneInformation_position = table.transform.position;
            SceneInformationManager.CrossSceneInformation_rotation = table.transform.rotation;
        }
    }
}
