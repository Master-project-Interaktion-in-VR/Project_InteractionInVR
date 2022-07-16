using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationAction : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float hapticIntensity;
    [SerializeField] private float hapticDuration;

    public void Teleport(TeleportingEventArgs args)
    {
        StartCoroutine(FadeCoroutine());
        audioSource.Play();
        args.interactorObject.transform.GetComponent<XRBaseController>().SendHapticImpulse(hapticIntensity, hapticDuration);
    }

    private IEnumerator FadeCoroutine()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeDuration);
        fadeScreen.FadeIn(fadeDuration);
    }
}
