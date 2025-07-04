using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialIntrolPanel : BaseTutorialPanel
{
    [Header("UI Elements")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle showIntroToggle;
    [SerializeField] private Toggle allowRestartToggle;

    [Header("Settings Fields (Auto-synced with IntroManager)")]
    [SerializeField] private bool showIntroOnStart = true;
    [SerializeField] private bool allowIntroRestart = true;

    [Header("TTS")]
    [SerializeField] private string TTSText = "";
    [SerializeField] private TTSSpeaker ttsSpeaker;

    private void Start()
    {
        AutoAssignComponents();
        SetupTTS();
        SetupButtons();
        SetupToggles();
        SyncFieldsWithIntroManager();
    }

    private void AutoAssignComponents()
    {
        // Auto-assign TTSSpeaker
        if (ttsSpeaker == null)
            ttsSpeaker = GetComponentInChildren<TTSSpeaker>();
        if (ttsSpeaker == null)
            ttsSpeaker = Object.FindFirstObjectByType<TTSSpeaker>();

        // Auto-assign buttons
        if (nextButton == null)
        {
            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                if (btn.name.ToLower().Contains("next"))
                {
                    nextButton = btn;
                    break;
                }
            }
        }

        if (skipButton == null)
        {
            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                if (btn.name.ToLower().Contains("skip"))
                {
                    skipButton = btn;
                    break;
                }
            }
        }

        if (closeButton == null)
        {
            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                if (btn.name == "DestructiveButton_IconAndLabel_UnityUIButton")
                {
                    closeButton = btn;
                    break;
                }
            }
        }

        // Auto-assign toggles
        if (showIntroToggle == null)
        {
            foreach (var toggle in GetComponentsInChildren<Toggle>(true))
            {
                if (toggle.name.ToLower().Contains("show") || toggle.name.ToLower().Contains("intro"))
                {
                    showIntroToggle = toggle;
                    break;
                }
            }
        }

        if (allowRestartToggle == null)
        {
            foreach (var toggle in GetComponentsInChildren<Toggle>(true))
            {
                if (toggle.name.ToLower().Contains("restart") || toggle.name.ToLower().Contains("allow"))
                {
                    allowRestartToggle = toggle;
                    break;
                }
            }
        }
    }

    private void SetupTTS()
    {
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Speak(TTSText);
        }
        else
        {
            Debug.LogWarning("TTSSpeaker not found on IntroPanel or in the scene.");
        }
    }

    private void SetupButtons()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButton);
        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButton);
        if (closeButton != null)
            closeButton.onClick.AddListener(OnSkipButton);
    }

    private void SetupToggles()
    {
        if (showIntroToggle != null)
        {
            showIntroToggle.onValueChanged.AddListener(OnShowIntroToggleChanged);
        }

        if (allowRestartToggle != null)
        {
            allowRestartToggle.onValueChanged.AddListener(OnAllowRestartToggleChanged);
        }
    }

    private void SyncFieldsWithIntroManager()
    {
        if (IntroManager.Instance != null)
        {
            // Get current values from IntroManager
            showIntroOnStart = IntroManager.Instance.GetShowIntroOnStart();
            allowIntroRestart = IntroManager.Instance.GetAllowIntroRestart();

            // Update toggle states to match
            if (showIntroToggle != null)
                showIntroToggle.isOn = showIntroOnStart;

            if (allowRestartToggle != null)
                allowRestartToggle.isOn = allowIntroRestart;

            Debug.Log($"Synced fields - ShowIntro: {showIntroOnStart}, AllowRestart: {allowIntroRestart}");
        }
    }

    private void OnShowIntroToggleChanged(bool value)
    {
        showIntroOnStart = value;

        if (IntroManager.Instance != null)
        {
            IntroManager.Instance.SetShowIntroOnStart(value);
        }

        Debug.Log($"Show intro on start toggled to: {value}");
    }

    private void OnAllowRestartToggleChanged(bool value)
    {
        allowIntroRestart = value;

        if (IntroManager.Instance != null)
        {
            IntroManager.Instance.SetAllowIntroRestart(value);
        }

        Debug.Log($"Allow intro restart toggled to: {value}");
    }

    // This is called by Unity when values change in the inspector
    private void OnValidate()
    {
        // Sync inspector changes to IntroManager if in play mode
        if (Application.isPlaying && IntroManager.Instance != null)
        {
            if (IntroManager.Instance.GetShowIntroOnStart() != showIntroOnStart)
            {
                IntroManager.Instance.SetShowIntroOnStart(showIntroOnStart);
                if (showIntroToggle != null)
                    showIntroToggle.isOn = showIntroOnStart;
            }

            if (IntroManager.Instance.GetAllowIntroRestart() != allowIntroRestart)
            {
                IntroManager.Instance.SetAllowIntroRestart(allowIntroRestart);
                if (allowRestartToggle != null)
                    allowRestartToggle.isOn = allowIntroRestart;
            }
        }
    }
}