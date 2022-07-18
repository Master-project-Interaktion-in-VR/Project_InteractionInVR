using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblySceneManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> deactivateObjects;


    // TODO deactivate objects for PC
    private void Awake()
    {
        //if (!Application.isMobilePlatform)
        //{
        //    foreach (GameObject obj in deactivateObjects)
        //    {
        //        obj.SetActive(false);
        //    }
        //}
    }
}
