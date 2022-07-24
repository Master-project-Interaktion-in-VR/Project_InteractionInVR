using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackButtonPresses : MonoBehaviour
{
    TrackingManager trackingManager;

    private void Start()
    {
        trackingManager = Object.FindObjectOfType<TrackingManager>();
    }
    public void TrackSoundPress()
    {
        trackingManager.IncreaseButtonSound();
    }

    public void TrackVibPress()
    {
        trackingManager.IncreaseButtonVibration();
    }

    public void TrackTutorialPress()
    {
        trackingManager.IncreaseButtonAssemblyTutorial();
    }

    public void TrackResetDrawing()
    {
        trackingManager.IncreaseButtonResetDrawing();
    }
}
