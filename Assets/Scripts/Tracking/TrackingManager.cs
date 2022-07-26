using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TrackingManager : MonoBehaviour
{
    private TrackingObject trackingObject;
    const string url = "https://fixit-bot.up.railway.app/";

    private int buttonPressesSound = 0;
    private int buttonPressesVibration = 0;
    private int buttonPressesAssemblyTutorial = 0;
    private int buttonPressesResetDrawing = 0;
    private int buildTries;
    private bool usedAutomatedAssembly;

    private Stopwatch timeInEnvScene = new Stopwatch();
    private Stopwatch timeInAssemblyScene = new Stopwatch();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject); //makes TrackingManager accessible from all scenes
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "EnvironmentGameScene") //TODO: change to buildIndex
        {
            trackingObject = new TrackingObject();
            timeInEnvScene.Start();
        }

        if(scene.name == "AssemblyScene") //TODO: change to buildIndex 
        {
            trackingObject.SetTimeInEnvironmentScene(timeInEnvScene.Elapsed);
            timeInEnvScene.Stop();
            timeInAssemblyScene.Start();
        }

        if(scene.name == "EndScene") ////TODO: change to buildIndex 
        {
            trackingObject.SetTimeInAssemblyScene(timeInAssemblyScene.Elapsed);
            timeInAssemblyScene.Stop();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void IncreaseButtonSound()
    {
        this.buttonPressesSound += 1;
    }
    public void IncreaseButtonVibration()
    {
        this.buttonPressesVibration += 1;
    }
    public void IncreaseButtonAssemblyTutorial()
    {
        this.buttonPressesAssemblyTutorial += 1;
    }
    public void IncreaseButtonResetDrawing()
    {
        this.buttonPressesResetDrawing += 1;
    }
    public void SetBuildTries(int buildTries)
    {
        this.buildTries = buildTries;
    }

    public void SetUsedAutomatedAssembly(bool usedAutomatedAssembly)
    {
        this.usedAutomatedAssembly = usedAutomatedAssembly;
    }

    private void OnApplicationQuit()
    {
        trackingObject.SetButtonPresses(this.buttonPressesSound, this.buttonPressesVibration, this.buttonPressesAssemblyTutorial, this.buttonPressesResetDrawing);
        trackingObject.addTrackingToTrackings();
        trackingObject.SetBuildTries(buildTries);
        trackingObject.SetUsedAutomatedAssembly(usedAutomatedAssembly);

        StartCoroutine(Send());
    }

    public IEnumerator Send()
    {
        // Setup form responses
        UnityEngine.Debug.Log(trackingObject.getJsonString());

        using (UnityWebRequest www = UnityWebRequest.Put(url, trackingObject.getJsonString()))
        {
            UnityEngine.Debug.Log("sind in unity web request angekommen");
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError|| www.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.Log(www.error);
            }
            else
            {
                UnityEngine.Debug.Log("Form upload complete!");
            }
        }

        yield return new WaitForSeconds(0.0f);        
    }

}
