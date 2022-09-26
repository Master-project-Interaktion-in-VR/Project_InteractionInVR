using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// It's a class that is used to calibrate the table with the fixed marker
/// </summary>
public class Calibration : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> build_objects_Prefab;

    [SerializeField]
    private GameObject table_Prefab;

    private Transform CameraRig;

    //the controller on the hand
    private Transform handMarker;  
    
    //the fixed controller
    private static Transform fixedMarker; 

    private static GameObject antennaPieces;

    private static BuildManager manager;

    private static List<GameObject> disassembleButtons;

    public static GameObject table;

    private void Start()
    {
        CameraRig = GameObject.Find("MRTK-Quest_OVRCameraRig(Clone)").transform;
        handMarker = CameraRig.FindChildRecursive("RightControllerAnchor").transform;
        table = GameObject.Find("Table");

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

        if (!AssemblySceneManager.RUNNING_IN_TEST_ROOM) // if in test room, calibrate must be in Update
        {
            PreCalibrate();
        }
    }

    private void Update()
    {
        if (OVRInput.GetActiveController() == OVRInput.Controller.Touch)
        {
            //detect is button 'A' has been pressed
            if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) 
            {
                Calibrate(handMarker.position, handMarker.rotation, false);
            }
        }
    }

    /// <summary>
    /// tries to calibrate the table with the saved information from the calibration in the environment scene
    /// </summary>
    private void PreCalibrate()
    {
        // if the calibration has been done before, load the saved information
        Vector3 position = SceneInformationManager.CrossSceneInformation_position;
        Quaternion rotation = SceneInformationManager.CrossSceneInformation_rotation;
        if (position != null && rotation != null)
        {
            Calibrate(position, rotation, true);
        }
    }

    /// <summary>
    /// calibrates the table with the position of rotation the hand marker
    /// </summary>
    /// <param name="Vector3">position</param>
    /// <param name="Quaternion">rotation</param>
    /// <param name="moveTable">true if the table should be moved</param>
    private void Calibrate(Vector3 position, Quaternion rotation, bool moveTable)
    {
        // if the antenna pieces are not instantiated, instantiate them
        if (BuildManager.build_objects.Count == 0)
        {
            if (antennaPieces == null)
            {
                antennaPieces = PhotonNetwork.Instantiate("AntennaPieces", new Vector3(0, 0.8f, 0), Quaternion.identity);
                antennaPieces.GetComponent<NetworkHelper>().SetParent(table.transform);
            }

            foreach (GameObject buildObj_prefab in build_objects_Prefab)
            {
                Vector3 pos = buildObj_prefab.transform.position;
                pos.y += 1;
                GameObject obj = PhotonNetwork.Instantiate("AssemblyAntennaPieces/" + buildObj_prefab.name, pos, buildObj_prefab.transform.rotation);
                obj.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
                BuildManager.build_objects.Add(obj);
            }

            // add the listeners to the disassemble buttons
            AddDisassembleListeners();
        }

        // if table was already calibrated in the environment scene, move the table to the new position
        if (moveTable)
        {
            table.transform.position = position;
            table.transform.rotation = rotation;
        }
        else
        {
            // make fixed marker the parent of the table
            fixedMarker.transform.parent = null;
            table.transform.parent = fixedMarker.transform;

            // move the table to the position of the hand marker
            fixedMarker.transform.position = position;
            fixedMarker.transform.rotation = rotation;

            // make the table the parent of the fixed marker again
            table.transform.parent = null;
            fixedMarker.transform.parent = table.transform.parent;
        }
        // Debug.Log("Calibrate with position " + position + " and rotation " + rotation + " moveTable: " + moveTable);
    }

    /// <summary>
    /// Adds the disassemble listeners.
    /// </summary>
    private void AddDisassembleListeners()
    {
        disassembleButtons = GameObject.FindGameObjectsWithTag("DisassembleButton").ToList();
        manager = GameObject.Find("BuildManager").GetComponent<BuildManager>();

        disassembleButtons.Find(x => x.name == "Disassemble_Button").GetComponent<Interactable>().OnClick.AddListener(manager.DisassembleObjects);

        // add listener for each disassemble button
        AddListener("Disassemble_halterungsstange_Button", "Halterungsstange(Clone)");
        AddListener("Disassemble_schuessel_Button", "schuessel(Clone)");
        AddListener("Disassemble_mittelstange_Button", "mittelstange(Clone)");
        AddListener("Disassemble_bodenteil_Button", "bodenteil(Clone)");
        AddListener("Disassemble_seitenteil_unten_Button", "seitenteil_unten(Clone)");
        AddListener("Disassemble_seitenteil_oben_Button", "seitenteil_oben(Clone)");
    }

    /// <summary>
    /// method for adding a listener to a disassemble button which will respawn the object by name
    /// </summary>
    /// <param name="buttonName">The button name.</param>
    /// <param name="objectName">The object name.</param>
    private void AddListener(string buttonName, string objectName)
    {
        // add listener for the button in disassembleButtons
        disassembleButtons.Find(x => x.name == buttonName).GetComponent<Interactable>().OnClick.AddListener(() => manager.Respawn_object(objectName));
    }
}
