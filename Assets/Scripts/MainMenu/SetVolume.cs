using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    /// <summary>
    /// Set the global volume.
    /// </summary>
    public void SetLevel(float sliderValue)
    {
        audioMixer.SetFloat("OverallVolume", Mathf.Log10(sliderValue) * 20); // audio is logarithmic
    }

}
