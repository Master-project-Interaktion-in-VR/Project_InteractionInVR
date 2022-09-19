using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Debug Console that shows errors in VR. Attached to the right hand.
/// </summary>
public class DebugConsole : MonoBehaviour
{
    private TMPro.TextMeshPro _text;

    private Dictionary<string, string> _debugLogs;

    private void Start()
    {
        _debugLogs = new Dictionary<string, string>();
        _text = GetComponent<TMPro.TextMeshPro>();
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Callback for a Unity log message.
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (_debugLogs == null || _text == null)
            return;

        if (type == LogType.Error || type == LogType.Exception)
        {
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (_debugLogs.ContainsKey(debugKey))
            {
                _debugLogs[debugKey] = debugValue;
            }
            else
            {
                _debugLogs.Add(debugKey, debugValue);
            }
        }
        
        string displayText = "";
        foreach (KeyValuePair<string, string> log in _debugLogs)
        {
            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += log.Key + ": " + log.Value + "\n";
        }
        _text.text = displayText;
    }

    /// <summary>
    /// Write a line to the debug console.
    /// </summary>
    public void WriteLine(string line)
    {
        _text.text = _text.text + "\n" + line + "\n";
        Canvas.ForceUpdateCanvases();
    }
}
