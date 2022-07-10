using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GameSceneManager : MonoBehaviour
{
    [Header("PC Configuration")]

    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private Camera assistantCamera;


    void Start()
    {
#if !UNITY_EDITOR
        // do VR in Editor
        if (!Application.isMobilePlatform)
        {
            // is assistant
            GameObject cameraWrapper = GameObject.Find("Main Camera");
            //Destroy(cameraWrapper.GetComponent<Camera>()); // TODO does EXCEPTION in paint matter for drawing???????????
            //cameraWrapper.GetComponent<Camera>()
            Destroy(cameraWrapper.GetComponent<AudioListener>());
            Destroy(cameraWrapper.GetComponent<TrackedPoseDriver>());

            //GameObject left = GameObject.Find("LeftHand Controller");
            //Destroy(left.GetComponent<XRController>()); // TODO CANNOT DISABLE XRCONTROLLER!!!!!!!!!!! SHOULD NOT MATTER??
            //left.transform.DetachChildren();
            //Destroy(cameraWrapper.transform.GetChild(0).gameObject);

            GameObject vrEmulator = new GameObject("VREmulator");
            cameraWrapper.transform.SetParent(vrEmulator.transform);

            //foreach (GameObject obj in deactivateObjects)
            //{
            //    obj.SetActive(false);
            //}
            assistantCamera.gameObject.SetActive(true);

            //playspaceTransform = GameObject.Find("DefaultGazeCursor");
            //playerTransform = GameObject.Find("MixedRealityPlayspace");
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
