using UnityEngine;
using System.Collections.Generic;
using System;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executeQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        var obj = new GameObject("UnityMainThreadDispatcher");
        _instance = obj.AddComponent<UnityMainThreadDispatcher>();
        DontDestroyOnLoad(obj);
    }

    public static void Enqueue(Action action)
    {
        lock (_executeQueue)
            _executeQueue.Enqueue(action);
    }

    void Update()
    {
        lock (_executeQueue)
        {
            while (_executeQueue.Count > 0)
                _executeQueue.Dequeue().Invoke();
        }
    }
}