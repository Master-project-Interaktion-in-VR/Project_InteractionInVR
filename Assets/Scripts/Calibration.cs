using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    Transform CameraRig;
    Transform handMarker;  //the controller on the hand
    static Transform fixedMarker; //the fixed controller
    public GameObject table_Prefab;
    public static GameObject table;
    public List<GameObject> build_objects_Prefab;
    static GameObject antennaPieces;

    static BuildManager manager;
    static List<GameObject> disassembleButtons;

    void Start()
    {
        CameraRig = GameObject.Find("MRTK-Quest_OVRCameraRig(Clone)").transform;
        handMarker = CameraRig.FindChildRecursive("RightControllerAnchor").transform;
        table = GameObject.Find("Table");

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

        Vector3 position = SceneInformationManager.CrossSceneInformation_position;
        Quaternion rotation = SceneInformationManager.CrossSceneInformation_rotation;
        if (position != null && rotation != null)
        {
            Calibrate(position, rotation, true);
        }
    }

    void Update()
    {
        // fix for starting assembly scene directly with photon
        if (!PhotonNetwork.InRoom)
            return;

        if (OVRInput.GetActiveController() == OVRInput.Controller.Touch)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) //detect is button 'A' has been pressed
            {
                Calibrate(handMarker.position, handMarker.rotation, false);
            }
        }
    }

    public void Calibrate(Vector3 position, Quaternion rotation, bool moveTable)
    {
        //if(table == null)
        //{
        //    table = Instantiate(table_Prefab);
        //}

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
                GameObject obj = PhotonNetwork.Instantiate(buildObj_prefab.name, pos, buildObj_prefab.transform.rotation);
                obj.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
                BuildManager.build_objects.Add(obj);
            }

            AddDisassembleListeners();
        }

        if (moveTable)
        {
            table.transform.position = position;
            table.transform.rotation = rotation;
        }
        else
        {
            fixedMarker.transform.parent = null;
            table.transform.parent = fixedMarker.transform;

            fixedMarker.transform.position = position;

            fixedMarker.transform.rotation = rotation;

            table.transform.parent = null;
            fixedMarker.transform.parent = table.transform.parent;
        }
        Debug.Log("Calibrate with position " + position + " and rotation " + rotation + " moveTable: " + moveTable);
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
        AddListener("Disassemble_halterungsstange_Button", "halterungsstange(Clone)");
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
