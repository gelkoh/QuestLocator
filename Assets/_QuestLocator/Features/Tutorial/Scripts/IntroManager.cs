// Modified IntroManager to work with TutorialStateManager
using UnityEngine;
using UnityEngine.Events;

public class IntroManager : MonoBehaviour
{
    [Header("Intro Settings")]
    [SerializeField] private bool showIntroOnStart = true;
    [SerializeField] private bool allowIntroRestart = true;

    [Header("Events")]
    [SerializeField] private UnityEvent OnIntroStart;
    [SerializeField] private UnityEvent OnIntroComplete;
    [SerializeField] private UnityEvent OnSkipIntro;

    [Header("State Management")]
    public bool IsInIntro { get; private set; } = false;

    // References
    private TutorialStateManager tutorialStateManager;

    // Singleton pattern for easy access from other scripts
    public static IntroManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get reference to tutorial state manager
        tutorialStateManager = TutorialStateManager.Instance;
    }

    private void Start()
    {
        // Subscribe to tutorial events
        if (tutorialStateManager != null)
        {
            tutorialStateManager.OnTutorialStart.AddListener(OnTutorialStarted);
            tutorialStateManager.OnTutorialComplete.AddListener(OnTutorialCompleted);
            tutorialStateManager.OnTutorialSkipped.AddListener(OnTutorialSkipped);
        }

        CheckAndStartIntro();
    }

    private void CheckAndStartIntro()
    {
        if (showIntroOnStart)
        {
            StartIntro();
        }
        else
        {
            SetMainApplicationState();
        }
    }

    public void StartIntro()
    {
        Debug.Log("Starting intro tutorial...");
        IsInIntro = true;

        // Disable main application components during intro
        DisableMainApplicationFeatures();

        // Start the tutorial state manager
        if (tutorialStateManager != null)
        {
            tutorialStateManager.StartTutorial();
        }

        // Trigger intro start event
        OnIntroStart?.Invoke();
    }

    private void OnTutorialStarted()
    {
        // Called when tutorial state manager starts
        Debug.Log("Tutorial state manager started");
    }

    private void OnTutorialCompleted()
    {
        CompleteIntro();
    }

    private void OnTutorialSkipped()
    {
        SkipIntro();
    }

    public void CompleteIntro()
    {
        Debug.Log("Intro tutorial completed!");
        IsInIntro = false;

        EnableMainApplicationFeatures();
        OnIntroComplete?.Invoke();
        SetMainApplicationState();
    }

    public void SkipIntro()
    {
        Debug.Log("Intro tutorial skipped!");
        IsInIntro = false;

        EnableMainApplicationFeatures();
        OnSkipIntro?.Invoke();
        SetMainApplicationState();
    }

    private void DisableMainApplicationFeatures()
    {

    }

    private void EnableMainApplicationFeatures()
    {
        /*
        var barcodeScanner = FindFirstObjectByType<BarcodeScanner>();
        if (barcodeScanner != null) barcodeScanner.enabled = true;
        */
    }

    private void SetMainApplicationState()
    {
        Debug.Log("Setting main application state...");
    }

    public void RestartIntro()
    {
        if (allowIntroRestart || !IsInIntro)
        {
            StartIntro();
        }
        else
        {
            Debug.Log("Intro restart is disabled or already in intro");
        }
    }

    public void ToggleIntroOnStart()
    {
        showIntroOnStart = !showIntroOnStart;
        Debug.Log($"Show intro on start: {showIntroOnStart}");
    }

    public void SetIntroOnStart(bool value)
    {
        showIntroOnStart = value;
        Debug.Log($"Show intro on start set to: {showIntroOnStart}");
    }
}