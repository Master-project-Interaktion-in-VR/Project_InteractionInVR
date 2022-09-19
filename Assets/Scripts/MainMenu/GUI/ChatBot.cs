using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple simulation of a spacecraft onboard system posting messages
/// to the info panel and updating status lights.
/// </summary>
public class ChatBot : MonoBehaviour
{
    [SerializeField]
    private LightsPanel lightsPanel;

    [SerializeField]
    private float messageProbabilityPerSecond;

    [SerializeField]
    private List<string> arbitraryLines;

    private TextPanel _textPanel;
    private bool _ready;

    private void Awake()
    {
        _textPanel = GetComponent<TextPanel>();
    }

    private void OnEnable()
    {
        StartCoroutine(WelcomeMessage());
    }

    private void Update()
    {
        if (!_ready)
            return;

        if (Random.value < Time.deltaTime * messageProbabilityPerSecond)
            StartCoroutine(WriteRandomLine());
    }

    /// <summary>
    /// Print welcome message and turn on indicator lights gradually.
    /// </summary>
    private IEnumerator WelcomeMessage()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        _textPanel.WriteLine("Welcome Chief! The board systems are currently booting...");
        yield return new WaitForSeconds(Random.Range(2, 4));
        _textPanel.WriteLine("...");
        yield return new WaitForSeconds(Random.Range(3, 7));
        if (Random.value < 0.5)
        {
            _textPanel.WriteLine("The core system is up and running. We've got some issues starting the external protectors...");
        }
        else
        {
            lightsPanel.SetGreen(GUIConstants.IndicatorLight.PROTECTORS);
            _textPanel.WriteLine("The core system is up and running. No errors detected.");
        }
        yield return new WaitForSeconds(0.2f);
        lightsPanel.SetGreen(GUIConstants.IndicatorLight.CORE);
        yield return new WaitForSeconds(Random.Range(1, 2));
        lightsPanel.SetGreen(GUIConstants.IndicatorLight.AIR_LOCK);
        yield return new WaitForSeconds(0.5f);
        lightsPanel.SetGreen(GUIConstants.IndicatorLight.OXYGEN);

        _ready = true;
    }

    /// <summary>
    /// Post one of the predefined chat lines to the text panel
    /// and wait an arbitrary time.
    /// </summary>
    private IEnumerator WriteRandomLine()
    {
        _ready = false;
        if (arbitraryLines.Count <= 0)
            yield break;
        int randIndex = Random.Range(0, arbitraryLines.Count - 1);
        _textPanel.WriteLine(arbitraryLines[randIndex]);
        arbitraryLines.Remove(arbitraryLines[randIndex]);
        yield return new WaitForSeconds(Random.Range(7, 14));
        _ready = true;
    }
}
