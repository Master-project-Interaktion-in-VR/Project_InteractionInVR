using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    public void SetLevel(float sliderValue)
    {
        audioMixer.SetFloat("OverallVolume", Mathf.Log10(sliderValue) * 20); // audio is logarithmic
    }

}
