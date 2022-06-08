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

    // Start is called before the first frame update
    void Start()
    {
        collisions = new Queue<CollisionEvent>();
        build_objects = Instantiate(build_objects_Prefab);

        ToggleHandVisualisation handVisualisation = new ToggleHandVisualisation();
        handVisualisation.OnToggleHandJoint();

        GameObject.Find("Disassemble_Button").GetComponent<Interactable>().OnClick.AddListener(DisassembleObjects);
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

    public void AssembleObjects(GameObject object1, GameObject object2)
    {
        object1 = RemoveComponents(object1);
        object2 = RemoveComponents(object2);

        // create new object with rigidbody and objectManipulator 
        GameObject holdingObject = new GameObject();
        holdingObject.name = "holdingBody";
        holdingObject = AddComponents(holdingObject);

        // make two objects children of new object
        object1.transform.parent = holdingObject.transform;
        object2.transform.parent = holdingObject.transform;
    }

    public void DisassembleObjects()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("BuildObject");
        Destroy(build_objects);
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
            Destroy(obj.GetComponent<CollisionManager>());
            // remove tag
            obj.tag = "InitialObject";
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
        obj.AddComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
        // add Collision Manager
        obj.AddComponent<CollisionManager>();
        // add Build tag
        obj.tag = "BuildObject";

        return obj;
    }
}

