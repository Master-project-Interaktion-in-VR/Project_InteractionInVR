using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        _textPanel = GetComponent<TextPanel>();
    }

    private void OnEnable()
    {
        StartCoroutine(WelcomeMessage());
    }

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


    void Update()
    {
        if (!_ready)
            return;

        if (Random.value < Time.deltaTime * messageProbabilityPerSecond)
            StartCoroutine(WriteRandomLine());
    }

    private IEnumerator WriteRandomLine()
    {
        _ready = false;
        int randIndex = Random.Range(0, arbitraryLines.Count - 1);
        _textPanel.WriteLine(arbitraryLines[randIndex]);
        arbitraryLines.Remove(arbitraryLines[randIndex]);
        yield return new WaitForSeconds(Random.Range(7, 14));
        _ready = true;
    }
}
