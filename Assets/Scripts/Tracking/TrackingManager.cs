using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TrackingManager : MonoBehaviour
{
    private TrackingObject trackingObject;
    const string url = "https://fixit-bot.up.railway.app/"; //TODO: richtige URl einfügen

    private int buttonPressesSound = 0;
    private int buttonPressesVibration = 0;
    private int buttonPressesAssemblyTutorial = 0;
    private int buttonPressesResetDrawing = 0;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "EnvironmentGameScene") //TODO: change to buildIndex
        {
            Debug.Log("environment scene laoded");
            trackingObject = new TrackingObject(Time.time);
        }

        if(scene.name == "VR_Testing") //TODO: change to buildIndex 
        {
            trackingObject.SetTimesAfterAssemblyScene(Time.time);
        }

        if(scene.name == "EndScene") ////TODO: change to buildIndex 
        {
            trackingObject.SetTimesAfterEnvironmentScene(Time.time);
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

    private void OnApplicationQuit()
    {
        trackingObject.SetButtonPresses(this.buttonPressesSound, this.buttonPressesVibration, this.buttonPressesAssemblyTutorial, this.buttonPressesResetDrawing);
        trackingObject.addTrackingToTrackings();

        StartCoroutine(Send());
    }

    public IEnumerator Send()
    {
        // Setup form responses
        Debug.Log(trackingObject.getJsonString());

        using (UnityWebRequest www = UnityWebRequest.Put(url, trackingObject.getJsonString()))
        {
            Debug.Log("sind in unity web request angekommen");
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError|| www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }

        yield return new WaitForSeconds(0.0f);        
    }

}
