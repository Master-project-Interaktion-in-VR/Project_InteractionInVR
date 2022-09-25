using System;
using UnityEngine.UI;

public static class GUIConstants
{
    /// <summary>
    /// Types of lights on the lights panel.
    /// </summary>
    public enum IndicatorLight
    {
        CORE,
        PHOTON,
        AIR_LOCK,
        OXYGEN,
        PROTECTORS,
        OPERATOR,
        ASSISTANT
    }

    [Serializable]
    public struct Light
    {
        public IndicatorLight light;
        public Image image;
    }


    public static string PLATFORM_PC = "PC";
    public static string PLATFORM_VR = "VR";
    public static string MENU_SCENE = "StartScene";
}
