using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBot : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private float messageProbabilityPerSecond;

    [SerializeField]
    private List<string> arbitraryLines;

    private TMPro.TextMeshProUGUI _text;
    private bool _ready;

    void Awake()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(WelcomeMessage());
    }

    private IEnumerator WelcomeMessage()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        WriteLine("Welcome Chief! The board systems are currently booting...");
        yield return new WaitForSeconds(Random.Range(2, 4));
        WriteLine("...");
        yield return new WaitForSeconds(Random.Range(3, 7));
        if (Random.value < 0.5)
            WriteLine("The core system is up and running. We've got some issues starting the external protectors...");
        else
            WriteLine("The core system is up and running. No errors detected.");
        _ready = true;
    }

    private void WriteLine(string line)
    {
        _text.text = _text.text + "\n" + line + "\n";
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
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
        WriteLine(arbitraryLines[randIndex]);
        arbitraryLines.Remove(arbitraryLines[randIndex]);
        yield return new WaitForSeconds(Random.Range(7, 14));
        _ready = true;
    }
}
