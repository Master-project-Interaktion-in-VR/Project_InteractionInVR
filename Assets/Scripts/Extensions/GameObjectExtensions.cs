using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject FindObject(this GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Hack for finding disabled game objects that are root objects
    /// </summary>
    public static GameObject FindObject(string name)
    {
        GameObject[] gs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject g in gs)
        {
            if (g.name == name)
            {
                return g;
            }
        }
        return null;
    }
    
}
