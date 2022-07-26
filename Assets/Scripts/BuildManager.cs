using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    private TrackingManager trackingManager;
    [Serializable]
    public class AssemblySuccessUnityEvent : UnityEvent<bool>
    {
        public AssemblySuccessUnityEvent() { }
    }

    public class CollisionEvent
    {
        public GameObject object1;
        public GameObject object2;
        public Vector3 position;
    }

    public enum SnapID
    {
        NoneInHoldingBody = 0,
        BothInHoldingBody = 1,
        FirstInHoldingBody = 2,
        SecondInHoldingBody = 3,
        Default = 4,
    }

    public static SnapID snapID = SnapID.Default;

    struct AssembledBuildPoints
    {
        public GameObject buildPoint1 { get; set; }
        public GameObject buildPoint2 { get; set; }

        public AssembledBuildPoints(GameObject buildPoint1, GameObject buildPoint2)
        {
            this.buildPoint1 = buildPoint1;
            this.buildPoint2 = buildPoint2;
        }
    }

    public static Queue<CollisionEvent> collisions;

    public List<GameObject> build_objects_Prefab;
    public static List<GameObject> build_objects;

    public static List<GameObject> holdingObjects_List;
    static List<AssembledBuildPoints> assembledBuildPoints;

    public GameObject assembledAntenna_Prefab;
    public GameObject infoCanvas_Prefab;
    public GameObject dialog_Prefab;
    static GameObject dialog;

    [SerializeField]
    private AssemblySuccessUnityEvent assemblySuccess = new AssemblySuccessUnityEvent();

    static int buildTries = 0;
    const int maxTries = 3;

    static bool assembledAntenna = false;


    void Awake()
    {
        collisions = new Queue<CollisionEvent>();
        build_objects = new List<GameObject>();
        assembledBuildPoints = new List<AssembledBuildPoints>();

        ToggleHandVisualisation handVisualisation = new ToggleHandVisualisation();
        handVisualisation.OnToggleHandJoint();

        holdingObjects_List = new List<GameObject>();
    }

    private void Start()
    {
        trackingManager = FindObjectOfType<TrackingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collisions.Count > 0)
        {
            // get collision 
            CollisionEvent collision = collisions.Dequeue();
            // dequeue collision that was triggered from the second object of the first collision
            collisions.Dequeue();
            GameObject obj1 = collision.object1;
            GameObject obj2 = collision.object2;
            AssembleObjects(obj1, obj2);
        }
    }

    public void AssembleObjects(GameObject buildPoint1, GameObject buildPoint2)
    {
        AssembledBuildPoints assembledBuildPoint = new AssembledBuildPoints(buildPoint1, buildPoint2);
        assembledBuildPoints.Add(assembledBuildPoint);

        GameObject buildModel1 = buildPoint1.transform.parent.gameObject;

        if (holdingObjects_List == null)
        {
            holdingObjects_List = new List<GameObject>();
        }

        // switch snapPoints if object1 is not the new object
        if (holdingObjects_List.Count > 0 && buildModel1.transform.parent.name == "holdingBody")
        {
            SnapObjectsTogether(buildPoint2, buildPoint1);
        }
        else
        {
            SnapObjectsTogether(buildPoint1, buildPoint2);
        }
    }

    void SnapObjectsTogether(GameObject snapPoint1, GameObject snapPoint2)
    {
        // get the parent of the Building Point (actual object)
        GameObject buildModel1 = snapPoint1.transform.parent.gameObject;
        GameObject buildModel2 = snapPoint2.transform.parent.gameObject;

        Debug.Log("obj1: " + buildModel1 + " obj2: " + buildModel2);

        // turn snapPoint
        snapPoint1.transform.localEulerAngles = new Vector3(snapPoint1.transform.localEulerAngles.x, snapPoint1.transform.localEulerAngles.y, snapPoint1.transform.localEulerAngles.z + 180);

        GameObject parent1 = buildModel1.transform.parent.gameObject;
        GameObject parent2 = buildModel2.transform.parent.gameObject;

        // check parents of the buildObjects for right snapping
        switch (parent1.name.Substring(0, 11))
        {
            case "AntennaPiec":
                switch (parent2.name.Substring(0, 11))
                {
                    case "AntennaPiec":
                        snapID = SnapID.NoneInHoldingBody;
                        break;
                    case "holdingBody":
                        snapID = SnapID.SecondInHoldingBody;
                        break;
                }
                break;
            case "holdingBody":
                switch (parent2.name.Substring(0, 11))
                {
                    case "AntennaPiec":
                        snapID = SnapID.FirstInHoldingBody;
                        break;
                    case "holdingBody":
                        snapID = SnapID.BothInHoldingBody;
                        break;
                }
                break;
        }

        switch (snapID)
        {
            case SnapID.NoneInHoldingBody:
                GameObject newHoldingObject = PhotonNetwork.Instantiate("holdingBody", Vector3.zero, Quaternion.identity);
                //Microsoft.MixedReality.Toolkit.UI.ObjectManipulator om = newHoldingObject.AddComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
                //om.TwoHandedManipulationType = Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Move | Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Rotate;
                //om.AllowFarManipulation = false;
                newHoldingObject.GetComponent<NetworkHelper>().SetName($"holdingBody{holdingObjects_List.Count}");
                holdingObjects_List.Add(newHoldingObject);

                // do not snap the mittelstange, because its the bigger object 
                if (buildModel1.name == "mittelstange(Clone)")
                {
                    Snap(snapPoint2, buildModel2, snapPoint1);
                }
                else
                {
                    Snap(snapPoint1, buildModel1, snapPoint2);
                }

                // remove components 
                buildModel1.GetComponent<NetworkHelper>().RemoveComponents();
                buildModel2.GetComponent<NetworkHelper>().RemoveComponents();

                // make two objects children of new object
                buildModel1.GetComponent<NetworkHelper>().SetParent(newHoldingObject.transform);
                buildModel2.GetComponent<NetworkHelper>().SetParent(newHoldingObject.transform);
                break;
            case SnapID.BothInHoldingBody:
                // find the holdingObjects of buildModels and make it the main holdingObject
                GameObject snap_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel1.name));
                GameObject other_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel2.name));

                // do not snap the mittelstange, because its the bigger object 
                if (buildModel1.name == "mittelstange(Clone)")
                {
                    // find the holdingObjects of buildModels and make it the main holdingObject
                    snap_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel2.name));
                    other_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel1.name));

                    Snap(snapPoint2, snap_HoldingObject, snapPoint1);

                    buildModel2.GetComponent<NetworkHelper>().SetParent(other_HoldingObject.transform);
                    snapPoint2.transform.parent = buildModel2.transform;
                }
                else
                {
                    Snap(snapPoint1, snap_HoldingObject, snapPoint2);
                    buildModel1.GetComponent<NetworkHelper>().SetParent(other_HoldingObject.transform);
                    snapPoint1.transform.parent = buildModel1.transform;
                }
                // make all children of the snap_HoldingObject to children of the other_HoldingObject
                int childCount = snap_HoldingObject.transform.childCount;
                List<Transform> children = new List<Transform>();
                for (int i = 0; i < childCount; ++i)
                    children.Add(snap_HoldingObject.transform.GetChild(i));

                foreach (Transform child in children)
                    child.parent = other_HoldingObject.transform;

                // remove it from list
                holdingObjects_List.Remove(snap_HoldingObject);

                Destroy(snap_HoldingObject);
                break;
            case SnapID.FirstInHoldingBody:
                Snap(snapPoint2, buildModel2, snapPoint1);
                buildModel2.GetComponent<NetworkHelper>().SetParent(buildModel1.transform.parent);
                buildModel2.GetComponent<NetworkHelper>().RemoveComponents();
                break;
            case SnapID.SecondInHoldingBody:
                Snap(snapPoint1, buildModel1, snapPoint2);
                buildModel1.GetComponent<NetworkHelper>().SetParent(buildModel2.transform.parent);
                buildModel1.GetComponent<NetworkHelper>().RemoveComponents();
                break;
            case SnapID.Default:
                // do nothing
                break;
        }
        buildModel1.GetComponent<NetworkHelper>().SetPosition(buildModel1.transform);
        buildModel2.GetComponent<NetworkHelper>().SetPosition(buildModel2.transform);

        snapID = SnapID.Default;

        CheckAssembly();
    }

    /// <summary>
    /// moving the objects snapPoint to the goalPoint
    /// moving snapObject accordingly as a child of the snapPoint
    /// </summary>
    /// <param name="snapPoint"></param>
    /// <param name="snapObject"></param>
    public void Snap(GameObject snapPoint, GameObject snapObject, GameObject goalPoint)
    {
        // make buildPoint to Parent for moving object
        snapPoint.transform.parent = null;
        snapObject.transform.parent = snapPoint.transform;

        // snap objects together
        snapPoint.transform.position = goalPoint.transform.position;
        snapPoint.transform.rotation = goalPoint.transform.rotation;

        // make buildPoint child of the object again
        snapObject.transform.parent = null;
        snapPoint.transform.parent = snapObject.transform;
    }

    /// <summary>
    /// Disassembles the objects by destroying all build objects and instantiating them again
    /// </summary>
    public void DisassembleObjects()
    {
        if (assembledAntenna)
            return;

        DestroyAllBuildObjects();

        collisions = new Queue<CollisionEvent>();
        assembledBuildPoints = new List<AssembledBuildPoints>();
        holdingObjects_List = new List<GameObject>();
        build_objects = new List<GameObject>();

        GameObject antennaPieces = Calibration.table.transform.Find("AntennaPieces(Clone)").gameObject;
        foreach (GameObject buildObj_prefab in build_objects_Prefab)
        {
            Vector3 pos = buildObj_prefab.transform.localPosition;
            pos.y += 1f;
            pos += Calibration.table.transform.position;
            GameObject obj = PhotonNetwork.Instantiate(buildObj_prefab.name, pos, buildObj_prefab.transform.localRotation);
            obj.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
            build_objects.Add(obj);
        }
    }

    /// <summary>
    /// Checks if the buildObjects are assembled correctly 
    /// </summary>
    void CheckAssembly()
    {
        // check newest assembling
        AssembledBuildPoints newest_assembledBuildPoint = assembledBuildPoints.Last();
        bool newest_correctAssembling = CollisionManager.correct_AssembledBuildPoints.Contains((newest_assembledBuildPoint.buildPoint1.name, newest_assembledBuildPoint.buildPoint2.name));
        Debug.Log("--- " + newest_assembledBuildPoint.buildPoint1.name + " + " + newest_assembledBuildPoint.buildPoint2.name + (newest_correctAssembling == true ? " CORRECT ASSEMBLED!" : " NOT CORECT ASSEMBLED. Try again..."));

        // if not all objects are assembled do nothing
        if (holdingObjects_List[0].transform.childCount < 6)
            return;

        Debug.Log("--- you have assembled all objects - checking if assmbled correctly");

        // checking all assembled Build Points
        foreach (AssembledBuildPoints assembledBuildPoint in assembledBuildPoints)
        {
            bool correctAssembling = CollisionManager.correct_AssembledBuildPoints.Contains((assembledBuildPoint.buildPoint1.name, assembledBuildPoint.buildPoint2.name));
            if (!correctAssembling)
            {
                buildTries++;
                if (buildTries >= maxTries)
                {
                    dialog = Instantiate(dialog_Prefab);
                    dialog.transform.Find("TitleText").GetComponent<TextMeshPro>().text = "System Warning: Not correct assembled!";
                    dialog.transform.Find("DescriptionText").GetComponent<TextMeshPro>().text =
                        "You have tried to build the Antenna three times \n" +
                        "Do you need some Help?";

                    GameObject buttonNo = dialog.transform.Find("ButtonParent").Find("ButtonNo").gameObject;
                    buttonNo.GetComponent<Interactable>().OnClick.AddListener(() => Destroy(dialog));
                    GameObject buttonYes = dialog.transform.Find("ButtonParent").Find("ButtonYes").gameObject;
                    buttonYes.GetComponent<Interactable>().OnClick.AddListener(SpawnAssembledAntenna);
                    return;
                }

                string loseText = "NOT CORRECT ASSEMBLED. Try again..";
                Debug.Log(loseText);
                ShowTextForSeconds(loseText, 5);
                return;
            }
        }
        string winText = "WHOOO you have build the Antenna!";
        DestroyAllBuildObjects();
        assembledAntenna = true;
        Debug.Log(winText);
        ShowTextForSeconds(winText, 5);
        trackingManager.SetBuildTries(buildTries);
        assemblySuccess.Invoke(true);
    }

    /// <summary>
    /// Spawns the assembled antenna and destroyes every build object and holding object
    /// </summary>
    public void SpawnAssembledAntenna()
    {
        trackingManager.SetUsedAutomatedAssembly(true);
        DestroyAllBuildObjects();
        Vector3 pos = assembledAntenna_Prefab.transform.position;
        pos += Calibration.table.transform.position;
        GameObject assembledAntennaObject = PhotonNetwork.Instantiate(assembledAntenna_Prefab.name, pos, assembledAntenna_Prefab.transform.rotation);
        //assembledAntennaObject.GetComponent<NetworkHelper>().SetParent(Calibration.table.transform);
        Destroy(dialog);
        assembledAntenna = true;
        assemblySuccess.Invoke(true);
    }

    /// <summary>
    /// deletes an object by name and instantiates it again
    /// </summary>
    /// <param name="objectName">The object name.</param>
    public void Respawn_object(string objectName)
    {
        if (assembledAntenna)
            return;

        GameObject old_object = build_objects.Find(x => x.name == objectName);
        GameObject antennaPieces = Calibration.table.transform.Find("AntennaPieces(Clone)").gameObject;

        // create prefab name from objectname without "(Clone)"
        List<string> toBeInstantiated = new List<string>();
        toBeInstantiated.Add(objectName.Replace("(Clone)", ""));

        // destroy old object
        PhotonNetwork.Destroy(old_object);
        build_objects.Remove(old_object);

        Transform parent = old_object.transform.parent;
        // if the object is attached to one other object in a holdingBody, destroy it too
        if (parent != antennaPieces && parent.childCount == 2) // object still existing?
        {
            foreach (Transform child in parent)
            {
                if (child.name != objectName)
                {
                    toBeInstantiated.Add(child.name.Replace("(Clone)", ""));
                    PhotonNetwork.Destroy(child.gameObject);
                }
            }
            // destroy holding body
            PhotonNetwork.Destroy(parent.gameObject);
            holdingObjects_List.Remove(parent.gameObject);
        }

        foreach (string prefabName in toBeInstantiated)
        {
            GameObject prefab = build_objects_Prefab.Find(x => x.name.Contains(prefabName));
            //Vector3 pos = prefab.transform.localPosition;
            //pos.y += 0.5f;
            //pos += Calibration.table.transform.position;
            Vector3 pos = GameObject.Find("Table/SpawnPoint").transform.position;
            pos.y += 0.5f;
            GameObject new_object = PhotonNetwork.Instantiate(prefabName, pos, prefab.transform.rotation);
            new_object.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
            //build_objects_Prefab.Find(x => x.name == prefabName)
            build_objects.Add(new_object);
        }
    }

    public void DestroyAllBuildObjects()
    {
        foreach (GameObject buildObj in build_objects)
        {
            PhotonNetwork.Destroy(buildObj);
        }
        foreach (GameObject holdingObject in holdingObjects_List)
        {
            PhotonNetwork.Destroy(holdingObject);
        }
    }

    /// <summary>
    /// Shows the text for seconds on a plane in VR
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="seconds">number of seconds.</param>
    public void ShowTextForSeconds(string text, int seconds)
    {
        GameObject infoCanvas = Instantiate(infoCanvas_Prefab);
        infoCanvas.transform.Find("InfoText_VR").GetComponent<TextMeshPro>().text = text;

        IEnumerator coroutine = WaitAndDelete(seconds, infoCanvas);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitAndDelete(float waitTime, GameObject hideObject)
    {
        yield return new WaitForSeconds(waitTime);
        hideObject.SetActive(false);
    }
}

