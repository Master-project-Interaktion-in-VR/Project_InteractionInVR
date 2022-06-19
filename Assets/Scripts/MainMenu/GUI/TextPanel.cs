using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPanel : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private AudioTrigger messageAudio;

    private TMPro.TextMeshProUGUI _text;
    private float _lastWritten;

    private void Awake()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void WriteLine(string line)
    {
        _lastWritten = Time.time;
        _text.text = _text.text + "\n" + line + "\n";
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
        messageAudio.PlayAudio();
    }

    public float GetLastWrittenTime() => _lastWritten;
}
