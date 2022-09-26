using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It's a static class that holds information that can be accessed from any scene 
/// </summary>
public static class SceneInformationManager
{
    public static string CrossSceneInformation_string { get; set; }
    public static Vector3 CrossSceneInformation_position { get; set; }
    public static Quaternion CrossSceneInformation_rotation { get; set; }
}
