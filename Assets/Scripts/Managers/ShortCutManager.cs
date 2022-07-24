using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShortCutManager : MonoBehaviour
{
    private Scene scene;
    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKey(KeyCode.T))
            {
                if (scene.name == "EnvironmentGameScene")
                {
                    Debug.Log("EnvironmentScene skipped");
                    SceneManager.LoadScene("AssemblyScene");
                } else if (scene.name == "AssemblyScene")
                {
                    Debug.Log("AssemblyScene skipped");
                    SceneManager.LoadScene("EndScene");
                }
            }
        }
    }
}
