using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TrackingManager : MonoBehaviour
{
    [SerializeField]
    private TrackingObject trackingObject;

    [SerializeField]
    const string url = "https://fixit-bot.up.railway.app/";

    [SerializeField] private int buttonPressesSound = 0;
    [SerializeField] private int buttonPressesVibration = 0;
    [SerializeField] private int buttonPressesAssemblyTutorial = 0;
    [SerializeField] private int buttonPressesResetDrawing = 0;
    [SerializeField] private int buildTries;
    [SerializeField] private bool usedAutomatedAssembly;

    [SerializeField] private Stopwatch timeInEnvScene = new Stopwatch();
    [SerializeField] private Stopwatch timeInAssemblyScene = new Stopwatch();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject); //makes TrackingManager accessible from all scenes
    }

    /// <summary>
    /// When the scene is loaded, if the scene is the environment scene, start the timer for the environment
    /// scene. If the scene is the assembly scene, stop the timer for the environment scene and start the
    /// timer for the assembly scene. If the scene is the end scene, stop the timer for the assembly scene
    /// </summary>
    /// <param name="Scene">The scene that was loaded.</param>
    /// <param name="LoadSceneMode"></param>
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

    /// <summary>
    /// This function increases the number of button presses by one that trigger the VR player sound
    /// </summary>
    public void IncreaseButtonSound()
    {
        this.buttonPressesSound += 1;
    }
    /// <summary>
    /// This function increases the number of the vibration button presses by 1
    /// </summary>
    public void IncreaseButtonVibration()
    {
        this.buttonPressesVibration += 1;
    }
    /// <summary>
    /// This function increases the number of button presses on the assembly tutorial video by 1
    /// </summary>
    public void IncreaseButtonAssemblyTutorial()
    {
        this.buttonPressesAssemblyTutorial += 1;
    }
    /// <summary>
    /// This function increases the number of times the reset drawing button has been pressed by 1.
    /// </summary>
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

    /// <summary>
    /// When the application quits, the tracking object is updated with the number of button presses and the
    /// number of build tries. Then, the tracking object is added to the list of trackings and the tracking
    /// object is sent to the server
    /// </summary>
    private void OnApplicationQuit()
    {
        trackingObject.SetButtonPresses(this.buttonPressesSound, this.buttonPressesVibration, this.buttonPressesAssemblyTutorial, this.buttonPressesResetDrawing);
        trackingObject.addTrackingToTrackings();
        trackingObject.SetBuildTries(buildTries);
        trackingObject.SetUsedAutomatedAssembly(usedAutomatedAssembly);

        StartCoroutine(Send());
    }

    /// <summary>
    /// It takes the trackingObject and sends it to the server
    /// </summary>
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
