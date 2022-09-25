using System;
using UnityEngine;


[Serializable]
public class TrackingObject 
{
    [SerializeField] private string userID = "tbd";
    [SerializeField] private int buildTries = 0;
    [SerializeField] private bool usedAutomatedAssembly = false;
    [SerializeField] private string timeInEnvironmentScene;
    [SerializeField] private string timeInAssemblyScene;
    [SerializeField] private int buttonPressesSound = 0;
    [SerializeField] private int buttonPressesVibration = 0;
    [SerializeField] private int buttonPressesAssemblyTutorial = 0;
    [SerializeField] private int buttonPressesResetDrawing = 0;


    private string strJson;

    public void SetTimeInEnvironmentScene(TimeSpan timeInEnvironmentScene)
    {
        this.timeInEnvironmentScene = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            timeInEnvironmentScene.Hours, timeInEnvironmentScene.Minutes, timeInEnvironmentScene.Seconds,
            timeInEnvironmentScene.Milliseconds / 10);
    }

    public void SetTimeInAssemblyScene(TimeSpan timeInAssemblyScene)
    {
        this.timeInAssemblyScene = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            timeInAssemblyScene.Hours, timeInAssemblyScene.Minutes, timeInAssemblyScene.Seconds,
            timeInAssemblyScene.Milliseconds / 10);
    }

    public void SetButtonPresses(int buttonPressesSound, int buttonPressesVibration, int buttonPressesAssemblyTutorial, int buttonPressesResetDrawing)
    {
        UnityEngine.Debug.Log("button presses set");
        this.buttonPressesSound = buttonPressesSound;
        this.buttonPressesVibration = buttonPressesVibration;
        this.buttonPressesAssemblyTutorial = buttonPressesAssemblyTutorial;
        this.buttonPressesResetDrawing = buttonPressesResetDrawing;
    }

    public void SetBuildTries(int buildTries)
    {
        this.buildTries = buildTries;
    }

    public void SetUsedAutomatedAssembly(bool usedAutomatedAssembly)
    {
        this.usedAutomatedAssembly = usedAutomatedAssembly;
    }

    public void addTrackingToTrackings()
    {
       strJson = JsonUtility.ToJson(this);
    }

    public string getJsonString()
    {
        return this.strJson;
    }
}
