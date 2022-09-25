using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Special menu panel to hold text messages.
/// </summary>
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

    /// <summary>
    /// Write a line to the text panel.
    /// </summary>
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
