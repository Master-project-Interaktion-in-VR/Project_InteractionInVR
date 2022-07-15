using UnityEngine;

public class CSVManager : MonoBehaviour
{
    private static string finalString = "";
    private static string[] headers = new string[8]
    {
        "assemblyAttempts",
        "usedAutomatedAssembly",
        "timeInEnvironmentScene",
        "timeInAssemblyScene",
        "buttonPressesSound",
        "buttonPressesVibration",
        "buttonPressesAssemblyTutorial",
        "buttonPressesResetDrawing"
    };

    public static void AddTrackingObject(string trackingObject)
    {
        finalString += trackingObject;
    }
    public static void ExportTrackingObjects()
    {
        string headerString = string.Join(";", headers) + "\n";
        finalString = headerString + finalString;
    }
    public static string GetFinalString()
    {
        return finalString;
    }
}
