using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DebugMenuScript : MonoBehaviour
{
    public TextMeshProUGUI ErrorTF;
    string output = "";
    string stack = "";

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        Debug.LogError("Debug Menu aktiviert");
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        ClearLog();

    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            output += logString + "\n";
        }
        ErrorTF.text = output;

    }
    
    public void CreateError()
    {
        Debug.LogError("errorButton");
    }
    public void ClearLog()
    {
        output = "";
        stack = "";
        ErrorTF.text = "";
    }
}
