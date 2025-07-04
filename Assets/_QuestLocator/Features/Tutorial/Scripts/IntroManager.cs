// Modified IntroManager to work with TutorialStateManager and persistent settings
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IntroManager : MonoBehaviour
{
    [Header("Intro Settings")]
    [SerializeField] private bool showIntroOnStart = true;  // Default value
    [SerializeField] private bool allowIntroRestart = true; // Default value

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

    // PlayerPrefs keys
    private const string SHOW_INTRO_KEY = "ShowIntroOnStart";
    private const string ALLOW_RESTART_KEY = "AllowIntroRestart";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Ensure this GameObject is a root object before calling DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);

            // Load persistent settings
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get reference to tutorial state manager
        tutorialStateManager = TutorialStateManager.TutorialStateManagerInstance;
        Debug.Log($"[IntroManager] tutorialStateManager is {(tutorialStateManager == null ? "null" : "assigned")}");
    }

    private void LoadSettings()
    {
        // Load from PlayerPrefs, use serialized field values as defaults
        showIntroOnStart = PlayerPrefs.GetInt(SHOW_INTRO_KEY, showIntroOnStart ? 1 : 0) == 1;
        allowIntroRestart = PlayerPrefs.GetInt(ALLOW_RESTART_KEY, allowIntroRestart ? 1 : 0) == 1;

        Debug.Log($"[IntroManager] Loaded settings - ShowIntro: {showIntroOnStart}, AllowRestart: {allowIntroRestart}");
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(SHOW_INTRO_KEY, showIntroOnStart ? 1 : 0);
        PlayerPrefs.SetInt(ALLOW_RESTART_KEY, allowIntroRestart ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"[IntroManager] Saved settings - ShowIntro: {showIntroOnStart}, AllowRestart: {allowIntroRestart}");
    }

    // Public getters and setters
    public bool GetShowIntroOnStart()
    {
        return showIntroOnStart;
    }

    public void SetShowIntroOnStart(bool value)
    {
        showIntroOnStart = value;
        SaveSettings();
        Debug.Log($"Show intro on start set to: {showIntroOnStart}");
    }

    public bool GetAllowIntroRestart()
    {
        return allowIntroRestart;
    }

    public void SetAllowIntroRestart(bool value)
    {
        allowIntroRestart = value;
        SaveSettings();
        Debug.Log($"Allow intro restart set to: {allowIntroRestart}");
    }

    private void Start()
    {
        // Subscribe to tutorial events - only the ones we need
        if (tutorialStateManager != null)
        {
            tutorialStateManager.OnTutorialComplete.AddListener(OnTutorialCompleted);
            tutorialStateManager.OnTutorialSkipped.AddListener(OnTutorialSkipped);
        }

        StartCoroutine(DelayedStartIntro());
    }

    private IEnumerator DelayedStartIntro()
    {
        yield return new WaitForSeconds(1f);
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

        // Trigger intro start event
        OnIntroStart?.Invoke();

        // Start the tutorial state manager
        if (tutorialStateManager != null)
        {
            tutorialStateManager.StartTutorial();
        }
    }

    private void OnTutorialCompleted()
    {
        Debug.Log("Tutorial completed - ending intro");
        EndIntro(false); // false = completed, not skipped
    }

    private void OnTutorialSkipped()
    {
        Debug.Log("Tutorial skipped - ending intro");
        EndIntro(true); // true = skipped
    }

    private void EndIntro(bool wasSkipped)
    {
        IsInIntro = false;
        EnableMainApplicationFeatures();

        if (wasSkipped)
        {
            OnSkipIntro?.Invoke();
        }
        else
        {
            OnIntroComplete?.Invoke();
        }

        SetMainApplicationState();
    }

    // Public methods for manual control (if needed)
    public void CompleteIntro()
    {
        if (IsInIntro && tutorialStateManager != null)
        {
            tutorialStateManager.CompleteTutorial();
        }
    }

    public void SkipIntro()
    {
        if (IsInIntro && tutorialStateManager != null)
        {
            tutorialStateManager.SkipTutorial();
        }
    }

    private void DisableMainApplicationFeatures()
    {
        // Add any features to disable during intro
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
        SetShowIntroOnStart(!showIntroOnStart);
    }

    // Reset to defaults (useful for testing)
    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteKey(SHOW_INTRO_KEY);
        PlayerPrefs.DeleteKey(ALLOW_RESTART_KEY);
        LoadSettings();
        Debug.Log("Settings reset to defaults");
    }

    // Unity Editor sync - called when values change in inspector
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            // If running, immediately save any inspector changes to PlayerPrefs
            SaveSettings();
        }
        else
        {
            // In editor, load from PlayerPrefs when script loads
            LoadSettingsInEditor();
        }
    }

    private void LoadSettingsInEditor()
    {
        // Load saved values to sync inspector with PlayerPrefs
        showIntroOnStart = PlayerPrefs.GetInt(SHOW_INTRO_KEY, showIntroOnStart ? 1 : 0) == 1;
        allowIntroRestart = PlayerPrefs.GetInt(ALLOW_RESTART_KEY, allowIntroRestart ? 1 : 0) == 1;
    }
}