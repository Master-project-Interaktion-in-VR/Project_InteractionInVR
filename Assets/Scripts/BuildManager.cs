using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
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
    public static List <GameObject> build_objects;

    public static List<GameObject> holdingObjects_List;
    static List<AssembledBuildPoints> assembledBuildPoints;

    public GameObject assembledAntenna_Prefab;
    public GameObject infoCanvas_Prefab;
    public GameObject dialog_Prefab;
    static GameObject dialog;

    static int buildTries = 2;
    const int maxTries = 3;

    // Start is called before the first frame update
    void Start()
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
        if (buildModel1.transform.parent.name == "AntennaPieces" && buildModel2.transform.parent.name == "AntennaPieces" && GameObject.Find("holdingBody") != null)
            newHoldingBody = true;

        // if both objects of the collision are already in a collection of assembled objects, put all in one holdingObject
        if (buildModel1.transform.parent.name == "holdingBody" && buildModel2.transform.parent.name == "holdingBody")
        {
            // find the holdingObjects of buildModels and make it the main holdingObject
            GameObject snap_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel1.name));
            GameObject other_HoldingObject = holdingObjects_List.Find(x => x.transform.Find(buildModel2.name));

            // remove it from list
            holdingObjects_List.Remove(snap_HoldingObject);

            // make snapPoint parent of holdingObject
            snapPoint.transform.parent = null;
            snap_HoldingObject.transform.parent = snapPoint.transform;

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
        buildModel1 = RemoveComponents(buildModel1);
        buildModel2 = RemoveComponents(buildModel2);

        // create new object with rigidbody and objectManipulator 
        if (holdingObjects_List.Count == 0)
        {
            GameObject holdingObject = new GameObject();
            holdingObject.name = "holdingBody";
            holdingObject = AddComponents(holdingObject);
            holdingObjects_List.Add(holdingObject);
        }

        if (newHoldingBody)
        {
            GameObject newHoldingObject = new GameObject();
            newHoldingObject.name = "holdingBody";
            newHoldingObject = AddComponents(newHoldingObject);

            // make two objects children of new object
            buildModel1.transform.parent = newHoldingObject.transform;
            buildModel2.transform.parent = newHoldingObject.transform;
            holdingObjects_List.Add(newHoldingObject);

            CheckAssembly();

            return;
        }

        // make two objects children of new object
        buildModel1.transform.parent = holdingObjects_List[0].transform;
        buildModel2.transform.parent = holdingObjects_List[0].transform;

        CheckAssembly();
    }

    public void DisassembleObjects()
    {
        foreach(GameObject buildObj in build_objects)
        {
            Destroy(buildObj);
        }
        foreach (GameObject holdingObject in holdingObjects_List)
        {
            Destroy(holdingObject);
        }

        collisions = new Queue<CollisionEvent>();
        assembledBuildPoints = new List<AssembledBuildPoints>();
        holdingObjects_List = new List<GameObject>();

        GameObject antennaPieces = Calibration.table.transform.Find("AntennaPieces").gameObject;
        foreach (GameObject buildObj_prefab in build_objects_Prefab)
        {
            GameObject obj = Instantiate(buildObj_prefab, antennaPieces.transform);
            build_objects.Add(obj);
        }
    }
    
    void CheckAssembly()
    {
        // check newest assembling
        AssembledBuildPoints newest_assembledBuildPoint = assembledBuildPoints.Last();
        bool newest_correctAssembling = CollisionManager.correct_AssembledBuildPoints.Contains((newest_assembledBuildPoint.buildPoint1.name, newest_assembledBuildPoint.buildPoint2.name));
        Debug.Log("--- " + newest_assembledBuildPoint.buildPoint1.name + " + " + newest_assembledBuildPoint.buildPoint2.name + (newest_correctAssembling == true ? " CORRECT ASSEMBLED!" : " NOT CORECT ASSEMBLED. Try again..."));

        // if not all objects are assembled do nothing
        if (holdingObjects_List[0].transform.childCount < 8)
            return;

        Debug.Log("--- you have assembled all objects - checking if assmbled correctly");

        // checking all assembled Build Points
        foreach (AssembledBuildPoints assembledBuildPoint in assembledBuildPoints)
        {
            bool correctAssembling = CollisionManager.correct_AssembledBuildPoints.Contains((assembledBuildPoint.buildPoint1.name, assembledBuildPoint.buildPoint2.name));
            if (!correctAssembling)
            {
                buildTries++;
                if(buildTries >= maxTries) 
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
        Debug.Log(winText);
        ShowTextForSeconds(winText, 5);
    }

    public GameObject RemoveComponents(GameObject obj)
    {
        try
        {
            // remove RigidBody of object
            Destroy(obj.GetComponent<Rigidbody>());
            // remove ObjectManipulator of object
            Destroy(obj.GetComponent<Microsoft.MixedReality.Toolkit.UI.CursorContextObjectManipulator>());
            Destroy(obj.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>());
            // remove Collision Manager
            //Destroy(obj.GetComponent<CollisionManager>());
            // remove tag
            //obj.tag = "InitialObject";
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
        }

        return obj;
    }

    public GameObject AddComponents(GameObject obj)
    {
        // add rigidBody to Object
        obj.AddComponent<Rigidbody>();
        // add ObjectManipulator to object
        Microsoft.MixedReality.Toolkit.UI.ObjectManipulator om = obj.AddComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
        om.TwoHandedManipulationType = Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Move | Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Rotate;
        om.AllowFarManipulation = false;
        // add Collision Manager
        //obj.AddComponent<CollisionManager>();
        // add Build tag
        //obj.tag = "BuildObject";

        return obj;
    }

    public void SpawnAssembledAntenna()
    {
        foreach (GameObject buildObj in build_objects)
        {
            Destroy(buildObj);
        }
        foreach (GameObject holdingObject in holdingObjects_List)
        {
            Destroy(holdingObject);
        }
        Instantiate(assembledAntenna_Prefab);
        Destroy(dialog);
    }

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

