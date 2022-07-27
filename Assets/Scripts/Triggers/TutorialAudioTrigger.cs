using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAudioTrigger : MonoBehaviour
{
    private AudioSource tutorialAudio;
    public AudioClip tutorialAudio_3;

    public void Start()
    {
        tutorialAudio = GetComponent<AudioSource>();
    }
    public void PlayAudio()
    {
        tutorialAudio.Play();
    }


    void Update()
    {
        if (!tutorialAudio.isPlaying)
        {
            StartCoroutine(PlayNewAudio(tutorialAudio_3));
        }
    }

    public IEnumerator PlayNewAudio(AudioClip newClip)
    {
        yield return new WaitForSeconds(3);
        tutorialAudio.clip = newClip;
        tutorialAudio.Play();
    }

}