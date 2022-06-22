using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GUIConstants
{
    //public static string CORE = "core";
    //public static string PHOTON = "photon";
    //public static string AIR_LOCK = "air lock";
    //public static string OXYGEN = "oxygen";
    //public static string PROTECTORS = "protectors";

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
}
