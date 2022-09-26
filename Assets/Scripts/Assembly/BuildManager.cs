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

/// <summary>
/// The BuildManager class is responsible for assembling the antenna pieces 
/// </summary>
public class BuildManager : MonoBehaviour
{
    [SerializeField]
    private AssemblySuccessUnityEvent assemblySuccess = new AssemblySuccessUnityEvent();

    [SerializeField]
    private TrackingManager trackingManager;

    // a list that holds the prefabs of the antenna pieces
    [SerializeField]
    private List<GameObject> build_objects_Prefab;

    // the assembled antenna prefab
    [SerializeField]
    private GameObject assembledAntenna_Prefab;

    [SerializeField]
    private GameObject infoCanvas_Prefab;

    [SerializeField]
    private GameObject dialog_Prefab;

    private static GameObject dialog;

    private static List<GameObject> holdingObjects_List;

    private static List<AssembledBuildPoints> assembledBuildPoints;

    private static int buildTries = 0;

    private const int maxTries = 3;

    private static bool assembledAntenna = false;
    
    private static SnapID snapID = SnapID.Default;

    public static List<GameObject> build_objects;

    public static Queue<CollisionEvent> collisions;

    [Serializable]
    public class AssemblySuccessUnityEvent : UnityEvent<bool>
    {
        public AssemblySuccessUnityEvent() { }
    }

    // It's a class that holds information about a collision event 
    public class CollisionEvent
    {
        public GameObject object1;
        public GameObject object2;
        public Vector3 position;
    }

    // state of the build objects of the collision 
    public enum SnapID
    {
        NoneInHoldingBody = 0,
        BothInHoldingBody = 1,
        FirstInHoldingBody = 2,
        SecondInHoldingBody = 3,
        Default = 4,
    }

    // a struct for the build points of the objects that are assembled
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

    private void Awake()
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

    private void Update()
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

    /// <summary>
    /// assemble the objects that collided
    /// If the object is already in the holding list, then snap the new object to the old object,
    /// otherwise snap the old object to the new object.
    /// </summary>
    /// <param name="GameObject">buildPoint1 red collision point of the first object</param>
    /// <param name="GameObject">buildPoint2 red collision point of the second object</param>
    private void AssembleObjects(GameObject buildPoint1, GameObject buildPoint2)
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

    /// <summary>
    /// The function takes two objects and assembles them together
    /// It checks if they are in the same parent object. If they are
    /// not, it creates a new parent object and makes the two objects children of it. If they are, it
    /// makes the second object a child of the first object
    /// </summary>
    /// <param name="GameObject">snapPoint1, snapPoint2</param>
    /// <param name="GameObject">snapPoint1, snapPoint2</param>
    private void SnapObjectsTogether(GameObject snapPoint1, GameObject snapPoint2)
    {
        // get the parent of the Building Point (actual object)
        GameObject buildModel1 = snapPoint1.transform.parent.gameObject;
        GameObject buildModel2 = snapPoint2.transform.parent.gameObject;

        //Debug.Log("obj1: " + buildModel1 + " obj2: " + buildModel2);

        // turn snapPoint
        snapPoint1.transform.localEulerAngles = new Vector3(snapPoint1.transform.localEulerAngles.x, snapPoint1.transform.localEulerAngles.y, snapPoint1.transform.localEulerAngles.z + 180);

        // get the parent of the objects
        GameObject parent1 = buildModel1.transform.parent.gameObject;
        GameObject parent2 = buildModel2.transform.parent.gameObject;

        // check parents of the buildObjects and setting snapID for right snapping
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

        // snap the objects together depending on the snapID
        switch (snapID)
        {
            // if both objects are not in the holdingBody, create a new holdingBody and make the objects children of it
            case SnapID.NoneInHoldingBody:
                GameObject newHoldingObject = PhotonNetwork.Instantiate("holdingBody", Vector3.zero, Quaternion.identity);
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
            // if both objects are in the holdingBody, make the second object a child of the holdingBody the first object is in
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
            // if the first object is in the holdingBody, make the second object a child of the holdingBody the first object is in
            case SnapID.FirstInHoldingBody:
                Snap(snapPoint2, buildModel2, snapPoint1);
                buildModel2.GetComponent<NetworkHelper>().SetParent(buildModel1.transform.parent);
                buildModel2.GetComponent<NetworkHelper>().RemoveComponents();
                break;
            // if the second object is in the holdingBody, make the first object a child of the holdingBody the second object is in
            case SnapID.SecondInHoldingBody:
                Snap(snapPoint1, buildModel1, snapPoint2);
                buildModel1.GetComponent<NetworkHelper>().SetParent(buildModel2.transform.parent);
                buildModel1.GetComponent<NetworkHelper>().RemoveComponents();
                break;
            case SnapID.Default:
                // do nothing
                break;
        }
        // set position of the objects within the network
        buildModel1.GetComponent<NetworkHelper>().SetPosition(buildModel1.transform);
        buildModel2.GetComponent<NetworkHelper>().SetPosition(buildModel2.transform);

        // reset snapID
        snapID = SnapID.Default;

        // check if the antenna is complete
        CheckAssembly();
    }

    /// <summary>
    /// moving the objects snapPoint to the goalPoint
    /// moving snapObject accordingly as a child of the snapPoint
    /// </summary>
    /// <param name="snapPoint">the snapPoint of the Object</param>
    /// <param name="snapObject">the object</param>
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
        // if the antenna is already assembled, do nothing
        if (assembledAntenna)
            return;

        // destroy all build objects
        DestroyAllBuildObjects();

        collisions = new Queue<CollisionEvent>();
        assembledBuildPoints = new List<AssembledBuildPoints>();
        holdingObjects_List = new List<GameObject>();
        build_objects = new List<GameObject>();

        GameObject antennaPieces = Calibration.table.transform.Find("AntennaPieces(Clone)").gameObject;
        /* Instantiating the prefabs in the build_objects_Prefab list and adding them to the
        build_objects list. */
        foreach (GameObject buildObj_prefab in build_objects_Prefab)
        {
            Vector3 pos = buildObj_prefab.transform.localPosition;
            pos.y += 1f;
            pos += Calibration.table.transform.position;
            GameObject obj = PhotonNetwork.Instantiate("AssemblyAntennaPieces/" + buildObj_prefab.name, pos, buildObj_prefab.transform.localRotation);
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
        //Debug.Log("--- " + newest_assembledBuildPoint.buildPoint1.name + " + " + newest_assembledBuildPoint.buildPoint2.name + (newest_correctAssembling == true ? " CORRECT ASSEMBLED!" : " NOT CORECT ASSEMBLED. Try again..."));

        // if not all objects are assembled do nothing
        if (holdingObjects_List[0].transform.childCount < 6)
            return;

        // checking all assembled Build Points
        foreach (AssembledBuildPoints assembledBuildPoint in assembledBuildPoints)
        {
            bool correctAssembling = false;
            try
            {
                correctAssembling = CollisionManager.correct_AssembledBuildPoints.Contains((assembledBuildPoint.buildPoint1.name, assembledBuildPoint.buildPoint2.name));
            }
            catch (Exception e)
            {
                Debug.Log("catched exception in BuildManager CheckAssembly(): " + e.Message);
            }

            if (!correctAssembling)
            {
                // increment the number of wrong assembling
                buildTries++;
                // if the number of wrong assembling is higher than the max number of tries, ask the user if he wants to spawn the antenna
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
        string winText = "you have build the Antenna!";
        StartCoroutine(DespawnAfterSeconds(1f));
        assembledAntenna = true;
        Debug.Log(winText);
        //ShowTextForSeconds(winText, 5);
        trackingManager.SetBuildTries(buildTries);
        assemblySuccess.Invoke(true);
    }

    /// <summary>
    /// Wait for a certain amount of time, then destroy all the objects in the scene
    /// </summary>
    /// <param name="waitTime">The amount of time to wait before despawning the object.</param>
    private IEnumerator DespawnAfterSeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        DestroyAllBuildObjects();
    }

    /// <summary>
    /// Spawns the assembled antenna and destroyes every build object and holding object
    /// </summary>
    private void SpawnAssembledAntenna()
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
        // if the antenna is already assembled, do nothing
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

        // Instantiating the prefabs that are in the list toBeInstantiated
        foreach (string prefabName in toBeInstantiated)
        {
            GameObject prefab = build_objects_Prefab.Find(x => x.name.Contains(prefabName));
            Vector3 pos = GameObject.Find("Table/SpawnPoint").transform.position;
            pos.y += 0.5f;
            GameObject new_object = PhotonNetwork.Instantiate("AssemblyAntennaPieces/" + prefabName, pos, prefab.transform.rotation);
            new_object.GetComponent<NetworkHelper>().SetParent(antennaPieces.transform);
            //build_objects_Prefab.Find(x => x.name == prefabName)
            build_objects.Add(new_object);
        }
    }

    /// <summary>
    /// This function destroys all the objects that are in the build_objects list and the
    /// holdingObjects_List
    /// </summary>
    private void DestroyAllBuildObjects()
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
    private void ShowTextForSeconds(string text, int seconds)
    {
        GameObject infoCanvas = Instantiate(infoCanvas_Prefab);
        infoCanvas.transform.Find("InfoText_VR").GetComponent<TextMeshPro>().text = text;

        IEnumerator coroutine = WaitAndDelete(seconds, infoCanvas);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// Wait for a certain amount of time, then disable the object
    /// </summary>
    /// <param name="waitTime">The amount of time to wait before hiding the object.</param>
    /// <param name="GameObject">The object you want to hide.</param>
    private IEnumerator WaitAndDelete(float waitTime, GameObject hideObject)
    {
        yield return new WaitForSeconds(waitTime);
        hideObject.SetActive(false);
    }
}

