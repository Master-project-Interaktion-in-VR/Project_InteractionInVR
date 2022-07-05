using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationAction : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float hapticIntensity;
    [SerializeField] private float duration;

    public void Teleport(TeleportingEventArgs args)
    {
        audioSource.Play();
        args.interactorObject.transform.GetComponent<XRBaseController>().SendHapticImpulse(hapticIntensity, duration);
    }
}
