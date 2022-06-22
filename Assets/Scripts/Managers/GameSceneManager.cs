using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            foreach (GameObject obj in deactivateObjects)
            {
                obj.SetActive(false);
            }
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
