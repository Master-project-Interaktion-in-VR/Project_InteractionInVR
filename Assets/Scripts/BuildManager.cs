using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public class CollisionEvent
    {
        public GameObject object1;
        public GameObject object2;
        public Vector3 position;
    }

    public static Queue<CollisionEvent> collisions;

    public GameObject build_objects_Prefab;
    GameObject build_objects;
    static List<GameObject> holdingObjects_List;

    // Start is called before the first frame update
    void Start()
    {
        collisions = new Queue<CollisionEvent>();
        build_objects = Instantiate(build_objects_Prefab);

        ToggleHandVisualisation handVisualisation = new ToggleHandVisualisation();
        handVisualisation.OnToggleHandJoint();

        GameObject.Find("Disassemble_Button").GetComponent<Interactable>().OnClick.AddListener(DisassembleObjects);

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
        GameObject buildModel1 = buildPoint1.transform.parent.gameObject;

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

        Debug.Log("object1: " + buildModel1 + " object2: " + buildModel2);

        // turn snapPoint
        snapPoint.transform.localEulerAngles = new Vector3(snapPoint.transform.localEulerAngles.x, snapPoint.transform.localEulerAngles.y, snapPoint.transform.localEulerAngles.z + 180);

        Debug.Log("---buildModel1 parent: " + buildModel1.transform.parent + " buildModel2 parent: " + buildModel2.transform.parent);
        bool newHoldingBody = false;
        bool combineBody = false;
        // if there are already assembled objects in a holding body, make a new one
        if (buildModel1.transform.parent.name == "AntennaPieces(Clone)" && buildModel2.transform.parent.name == "AntennaPieces(Clone)" && GameObject.Find("holdingBody") != null)
            newHoldingBody = true;

        // if both objects of the collision are already in a collection of assembled objects, put all in one holdingObject
        if (buildModel1.transform.parent.name == "holdingBody" && buildModel2.transform.parent.name == "holdingBody")
        {
            combineBody = true;
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
            // TODO: Bug fixen children get deleted
            int childCount = transform.childCount;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < childCount; ++i)
                children.Add(transform.GetChild(i));

            foreach (Transform child in children)
                child.parent = other_HoldingObject.transform;

            Destroy(snap_HoldingObject);
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
            Debug.Log("---new");
            GameObject newHoldingObject = new GameObject();
            newHoldingObject.name = "holdingBody";
            newHoldingObject = AddComponents(newHoldingObject);

            // make two objects children of new object
            buildModel1.transform.parent = newHoldingObject.transform;
            buildModel2.transform.parent = newHoldingObject.transform;
            holdingObjects_List.Add(newHoldingObject);
            return;
        }

        //if (combineBody)
        //{
        //    Debug.Log("---combineBody");
        //    return;
        //}

        // make two objects children of new object
        buildModel1.transform.parent = holdingObjects_List[0].transform;
        buildModel2.transform.parent = holdingObjects_List[0].transform;
        Debug.Log("---old");
    }

    public void DisassembleObjects()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("InitialObject");
        Destroy(build_objects);
        foreach(GameObject holdingObject in holdingObjects_List)
        {
            Destroy(holdingObject);
        }
        foreach(GameObject obj in objects)
        {
            Destroy(obj);
        }
        build_objects = Instantiate(build_objects_Prefab);
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
}

