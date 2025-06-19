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
        output = logString + "\n" + output;

        stack = stackTrace;
        ErrorTF.text = output;
    }
    public void ClearLog()
    {
        ErrorTF.text = "";
    }
    private void OnGUI()
    {
        
    }
    public void CreateError()
    {
        Debug.LogError("errorButton");
    }

}
