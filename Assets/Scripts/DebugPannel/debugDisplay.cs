using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class debugDisplay : MonoBehaviour
{
    public TextMeshProUGUI display;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI errorText;
    public int maxLines = 15; // Max number of lines to show

    private Queue<string> logQueue = new Queue<string>();
    private Queue<string> errorQueue = new Queue<string>();
    void Update()
    {
        timeText.text = Time.time.ToString("0.00"); 
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Log && type != LogType.Error) return;

        // Add log to the queue
        if (logQueue.Count >= maxLines)
        {
            logQueue.Dequeue(); // Remove oldest log
        }
        if (errorQueue.Count >= maxLines)
        {
            errorQueue.Dequeue(); // Remove oldest log
        }

        if (type == LogType.Log) {
            logString = "[" + timeText.text + "]: " + logString;
            logQueue.Enqueue(logString);
            display.text = string.Join("\n", logQueue.ToArray());
        }
        else if (type == LogType.Error) {
            errorQueue.Enqueue(logString);
            errorText.text = string.Join("\n", errorQueue.ToArray());
        }
        // Rebuild display text

    }
}
