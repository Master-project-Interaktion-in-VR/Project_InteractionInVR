using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> deactivateObjects;

    [SerializeField]
    private Camera assistantCamera;


    // Start is called before the first frame update
    void Start()
    {
        if (SceneSpanningData.IsAssistant)
        {
            Debug.Log("I AM THE ASSISTANT");
            foreach (GameObject obj in deactivateObjects)
            {
                obj.SetActive(false);
            }
            assistantCamera.gameObject.SetActive(true);

            //playspaceTransform = GameObject.Find("DefaultGazeCursor");
            //playerTransform = GameObject.Find("MixedRealityPlayspace");
        }
        else
            Debug.Log("I AM THE PLAYER");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
