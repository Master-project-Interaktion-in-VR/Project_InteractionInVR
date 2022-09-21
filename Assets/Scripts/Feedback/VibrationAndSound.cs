using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Enable sound and vibration triggering over the network.
/// </summary>
public class VibrationAndSound : MonoBehaviour
{
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

    /// <summary>
    /// Corresponding RPC for triggering vibration.
    /// </summary>
    [PunRPC]
    public void TriggerVibrationRpc()
    {
        // only trigger vibration if detector in hand
        if (InventoryManager.DetectorIsInLeftHand())
            leftController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
        else if (InventoryManager.DetectorIsInRightHand())
            rightController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
        else
            return;

        Debug.Log("Trigger vibration");
    }

    /// <summary>
    /// Corresponding RPC for triggering sound feedback.
    /// </summary>
    [PunRPC]
    public void TriggerSoundRpc()
    {
        Debug.Log("Trigger sound");
        _detectorAudio.Play();
    }
}
