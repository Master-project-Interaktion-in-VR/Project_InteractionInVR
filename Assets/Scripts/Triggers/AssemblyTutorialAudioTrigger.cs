using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyTutorialAudioTrigger : MonoBehaviour
{
    private AudioSource tutorialAudio;
    public AudioClip tutorialAudio_10;

   private bool playAudio = true;

    // Update is called once per frame
    void Update()
    {
        if(OVRPlugin.GetHandTrackingEnabled() == true && playAudio)
        {
            tutorialAudio.Play();

            if (!tutorialAudio.isPlaying)
            {
                playAudio = false;
                StartCoroutine(PlayNewAudio(tutorialAudio_10));
            }
        }
    }

    public IEnumerator PlayNewAudio(AudioClip newClip)
    {
        yield return new WaitForSeconds(3);
        tutorialAudio.clip = newClip;
        tutorialAudio.Play();
    }
}
