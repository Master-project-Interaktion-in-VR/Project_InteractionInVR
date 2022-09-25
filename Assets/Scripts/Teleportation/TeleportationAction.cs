using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationAction : MonoBehaviour
{
    [SerializeField] 
    private AudioSource audioSource;
    
    [SerializeField] 
    private FadeScreen fadeScreen;
    
    [SerializeField] 
    private float fadeDuration;
    
    [SerializeField] 
    private float hapticIntensity;
    
    [SerializeField] 
    private float hapticDuration;
    
    /// <summary>
    /// When teleporting, the fade animation and an audio clip is played a haptic vibration is sent
    /// </summary>
    /// <param name="args"></param>
    public void Teleport(TeleportingEventArgs args)
    {
        StartCoroutine(FadeCoroutine());
        audioSource.Play();
        args.interactorObject.transform.GetComponent<XRBaseController>().SendHapticImpulse(hapticIntensity, hapticDuration);
    }

    /// <summary>
    /// Fade out and after fadeDuration fade in
    /// </summary>
    /// <returns> IEnumerator for Coroutine </returns>
    private IEnumerator FadeCoroutine()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeDuration);
        fadeScreen.FadeIn(fadeDuration);
    }
}
