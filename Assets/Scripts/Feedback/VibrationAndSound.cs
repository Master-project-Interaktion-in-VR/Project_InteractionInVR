using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VibrationAndSound : MonoBehaviour
{
    [SerializeField]
    private bool vibrationEnabled; // this does nothing. if the vib button is pressed in assembly, the event is simply not thrown by the UI

    [Header("Vibration Settings")]
    [SerializeField]
    private XRBaseController leftController;

    [SerializeField]
    private XRBaseController rightController;

    [SerializeField]
    private float vibrationDuration;

    [SerializeField]
    private float vibrationIntensity;

    [SerializeField]
    private InventoryManager InventoryManager;

    private AudioSource _detectorAudio;
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _detectorAudio = GetComponent<AudioSource>();

    }

    /// <summary>
    /// Trigger the vibration for the controller of the VR player
    /// </summary>
    public void TriggerVibration()
    {
        _photonView.RPC("TriggerVibrationRpc", RpcTarget.All);
    }

    /// <summary>
    /// Trigger the sound for every player.
    /// </summary>
    public void TriggerSound()
    {
        _photonView.RPC("TriggerSoundRpc", RpcTarget.All);
    }


    [PunRPC]
    public void TriggerVibrationRpc()
    {
        // only trigger sound if detector in hand
        if (InventoryManager.DetectorIsInLeftHand())
            leftController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
        else if (InventoryManager.DetectorIsInRightHand())
            rightController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
        else
            return;

        Debug.Log("Trigger vibration");
    }


    [PunRPC]
    public void TriggerSoundRpc()
    {
        Debug.Log("Trigger sound");
        _detectorAudio.Play();
    }
}
