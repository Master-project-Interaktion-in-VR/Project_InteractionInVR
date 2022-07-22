using System;
using UnityEngine;


[Serializable]
public class TrackingObject 
{
    [SerializeField] private string userID = "tbd";
    [SerializeField] private int assemblyAttempts = 0; //TODO: implement
    [SerializeField] private bool usedAutomatedAssembly = false; //TODO: implement
    [SerializeField] private float timeInEnvironmentScene = 0f;
    [SerializeField] private float timeInAssemblyScene = 0f;
    [SerializeField] private int buttonPressesSound = 0;
    [SerializeField] private int buttonPressesVibration = 0;
    [SerializeField] private int buttonPressesAssemblyTutorial = 0;
    [SerializeField] private int buttonPressesResetDrawing = 0;

    private float startTimeAssemblyScene;
    private float startTimeEnvironmentScene;

    private string strJson;

    public void SetTimesAfterAssemblyScene(float endTimeAssemblyScene)
    {
        this.timeInAssemblyScene = (this.startTimeAssemblyScene - endTimeAssemblyScene) * 1000; //TODO: maybe time in seconds?
        this.startTimeEnvironmentScene = endTimeAssemblyScene;
    }

    public void SetTimesAfterEnvironmentScene(float endTimeEnvironmentScene)
    {
        this.timeInEnvironmentScene = (this.startTimeEnvironmentScene - endTimeEnvironmentScene) * 1000; //TODO: maybe time in seconds?
    }

    public void SetButtonPresses(int buttonPressesSound, int buttonPressesVibration, int buttonPressesAssemblyTutorial, int buttonPressesResetDrawing)
    {
        UnityEngine.Debug.Log("button presses set");
        this.buttonPressesSound = buttonPressesSound;
        this.buttonPressesVibration = buttonPressesVibration;
        this.buttonPressesAssemblyTutorial = buttonPressesAssemblyTutorial;
        this.buttonPressesResetDrawing = buttonPressesResetDrawing;
    }

    public TrackingObject(float startTimeAssemblyScene)
    {
        this.startTimeAssemblyScene = startTimeAssemblyScene;
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
