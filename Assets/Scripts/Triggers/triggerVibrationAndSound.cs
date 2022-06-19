using UnityEngine;

public class triggerVibrationAndSound : MonoBehaviour
{
    private AudioSource audioSource;
    public void triggerVibration()
    {
        Debug.Log("trigger vibration");
        // starts vibration on the right Touch controller
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
    }

    public void triggerSound()
    {
        Debug.Log("trigger sound");
        audioSource = GameObject.Find("TriggerVibrationAndSound").GetComponent<AudioSource>();
        audioSource.Play();
    }
}
