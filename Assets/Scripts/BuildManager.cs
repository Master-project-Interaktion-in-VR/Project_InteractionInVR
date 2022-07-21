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

    public AssemblySuccessUnityEvent AssemblySuccess = new AssemblySuccessUnityEvent();

    static int buildTries = 3;
    const int maxTries = 3;

    static bool assembledAntenna = false;

    // Start is called before the first frame update
    void Awake()
    {
        collisions = new Queue<CollisionEvent>();
        build_objects = new List<GameObject>();
        assembledBuildPoints = new List<AssembledBuildPoints>();

        ToggleHandVisualisation handVisualisation = new ToggleHandVisualisation();
        handVisualisation.OnToggleHandJoint();

        holdingObjects_List = new List<GameObject>();
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

    void SnapObjectsTogether(GameObject snapPoint, GameObject otherPoint)
    {
        // get the parent of the Building Point (actual object)
        GameObject buildModel1 = snapPoint.transform.parent.gameObject;
        GameObject buildModel2 = otherPoint.transform.parent.gameObject;

        // turn snapPoint
        snapPoint.transform.localEulerAngles = new Vector3(snapPoint.transform.localEulerAngles.x, snapPoint.transform.localEulerAngles.y, snapPoint.transform.localEulerAngles.z + 180);

        bool newHoldingBody = false;
        // if there are already assembled objects in a holding body, make a new one
        if (buildModel1.transform.parent.name == "AntennaPieces(Clone)" && buildModel2.transform.parent.name == "AntennaPieces(Clone)" && GameObject.Find("holdingBody") != null)
            newHoldingBody = true;

        // if both objects of the collision are already in a collection of assembled objects, put all in one holdingObject
        if (buildModel1.transform.parent.name.Substring(0, 11) == "holdingBody" && buildModel2.transform.parent.name.Substring(0, 11) == "holdingBody")
        {
            // find the holdingObjects of buildModels and make it the main holdingObject
            GameObject snap_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel1.name));
            GameObject other_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel2.name));

            // remove it from list
            holdingObjects_List.Remove(snap_HoldingObject);

            // make snapPoint parent of holdingObject
            snapPoint.transform.parent = null;
            snap_HoldingObject.transform.parent = snapPoint.transform;
            ////snapPoint.GetComponent<AntennaPiece>().SetParentRoot();
            //snap_HoldingObject.GetComponent<AntennaPiece>().SetParent(snapPoint.transform);

            // snap objects together
            snapPoint.transform.position = otherPoint.transform.position;
            snapPoint.transform.rotation = otherPoint.transform.rotation;

            // make snapPoint child of BuildModel1
            snap_HoldingObject.transform.parent = null;
            snapPoint.transform.parent = buildModel1.transform;

            // make all children of the snap_HoldingObject to children of the other_HoldingObject
            int childCount = snap_HoldingObject.transform.childCount;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < childCount; ++i)
                children.Add(snap_HoldingObject.transform.GetChild(i));

            foreach (Transform child in children)
                child.parent = other_HoldingObject.transform;

            Destroy(snap_HoldingObject);

            buildModel1.GetComponent<NetworkHelper>().SetPosition(buildModel1.transform);
            buildModel2.GetComponent<NetworkHelper>().SetPosition(buildModel2.transform);

            CheckAssembly();

            return;
        }

        // make buildPoint to Parent for moving object
        snapPoint.transform.parent = null;
        buildModel1.transform.parent = snapPoint.transform;

        // snap objects together
        snapPoint.transform.position = otherPoint.transform.position;
        snapPoint.transform.rotation = otherPoint.transform.rotation;

        // make buildPoint child of the object again
        buildModel1.transform.parent = null;
        snapPoint.transform.parent = buildModel1.transform;

        // remove components 
        buildModel1.GetComponent<NetworkHelper>().RemoveComponents();
        buildModel2.GetComponent<NetworkHelper>().RemoveComponents();

        // create new object with rigidbody and objectManipulator 
        if (holdingObjects_List.Count == 0)
        {
            GameObject holdingObject = PhotonNetwork.Instantiate("HoldingBody", Vector3.zero, Quaternion.identity);
            holdingObject.GetComponent<NetworkHelper>().InitHoldingBody();
            holdingObjects_List.Add(holdingObject);
        }

        if (newHoldingBody)
        {
            GameObject newHoldingObject = PhotonNetwork.Instantiate("HoldingBody", Vector3.zero, Quaternion.identity);
            newHoldingObject.GetComponent<NetworkHelper>().InitHoldingBody();
            newHoldingObject.name = $"holdingBody{holdingObjects_List.Count}";

            // make two objects children of new object
            buildModel1.GetComponent<NetworkHelper>().SetParent(newHoldingObject.transform);
            buildModel2.GetComponent<NetworkHelper>().SetParent(newHoldingObject.transform);
            holdingObjects_List.Add(newHoldingObject);

            buildModel1.GetComponent<NetworkHelper>().SetPosition(buildModel1.transform);
            buildModel2.GetComponent<NetworkHelper>().SetPosition(buildModel2.transform);

            CheckAssembly();

            return;
        }

        // make two objects children of new object
        buildModel1.GetComponent<NetworkHelper>().SetParent(holdingObjects_List[0].transform);
        buildModel2.GetComponent<NetworkHelper>().SetParent(holdingObjects_List[0].transform);

        buildModel1.GetComponent<NetworkHelper>().SetPosition(buildModel1.transform);
        buildModel2.GetComponent<NetworkHelper>().SetPosition(buildModel2.transform);

        CheckAssembly();
    }

    /// <summary>
    /// Disassembles the objects by destroying all build objects and instantiating them again
    /// </summary>
    public void DisassembleObjects()
    {
        if (assembledAntenna)
            return;

        foreach (GameObject buildObj in build_objects)
        {
            PhotonNetwork.Destroy(buildObj);
        }
        foreach (GameObject holdingObject in holdingObjects_List)
        {
            PhotonNetwork.Destroy(holdingObject);
        }

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
        assembledAntenna = true;
        Debug.Log(winText);
        ShowTextForSeconds(winText, 5);
        AssemblySuccess.Invoke(true);
    }

    /// <summary>
    /// Spawns the assembled antenna and destroyes every build object and holding object
    /// </summary>
    public void SpawnAssembledAntenna()
    {
        foreach (GameObject buildObj in build_objects)
        {
            PhotonNetwork.Destroy(buildObj);
        }
        foreach (GameObject holdingObject in holdingObjects_List)
        {
            PhotonNetwork.Destroy(holdingObject);
        }
        Vector3 pos = assembledAntenna_Prefab.transform.position;
        pos.y += 0.5f;
        pos += Calibration.table.transform.position;
        PhotonNetwork.Instantiate(assembledAntenna_Prefab.name, pos, assembledAntenna_Prefab.transform.rotation);
        Destroy(dialog);
        assembledAntenna = true;
        AssemblySuccess.Invoke(true);
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

        Transform parent = old_object.transform.parent;

        // reset position of the object
        PhotonNetwork.Destroy(old_object);
        build_objects.Remove(old_object);

        // if the object is attached to one other object in a holdingBody
        if (parent != antennaPieces && parent.childCount == 2)
        {
            // get all children of the parent
            List<GameObject> children = new List<GameObject>();
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; ++i)
                // add child from parent to list
                children.Add(parent.GetChild(i).gameObject);

            foreach (GameObject child in children)
            {
                if (child.name != objectName)
                {
                    // move child to the antennaPieces
                    child.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
                    child.GetComponent<NetworkHelper>().AddComponents();
                }
            }

            // destroy holding body
            PhotonNetwork.Destroy(parent.gameObject);
            holdingObjects_List.Remove(parent.gameObject);
        }

        // crete prefab name from objectname without "(Clone)"
        string prefabName = objectName.Replace("(Clone)", "");

        GameObject prefab = build_objects_Prefab.Find(x => x.name.Contains(prefabName));
        Vector3 pos = prefab.transform.localPosition;
        pos.y += 0.5f;
        pos += Calibration.table.transform.position;
        GameObject new_object = PhotonNetwork.Instantiate(prefabName, pos, prefab.transform.rotation);
        new_object.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
        //build_objects_Prefab.Find(x => x.name == prefabName)
        build_objects.Add(new_object);
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

