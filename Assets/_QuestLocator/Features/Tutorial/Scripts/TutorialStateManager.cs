using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class TutorialStateManager : MonoBehaviour
{
    [Header("Tutorial Panels")]
    [SerializeField] private BaseTutorialPanel[] tutorialPanels;

    [Header("Events")]
    public UnityEvent OnTutorialStart;
    public UnityEvent OnTutorialComplete;
    public UnityEvent OnTutorialSkipped;

    private int currentPanelIndex = -1;
    private bool isTutorialActive = false;
    private BaseTutorialPanel currentPanel;

    // Singleton for easy access
    public static TutorialStateManager Instance { get; private set; }

    public bool IsTutorialActive => isTutorialActive;
    public int CurrentStep => currentPanelIndex;
    public int TotalSteps => tutorialPanels.Length;

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

        InitializePanels();
    }

    private void InitializePanels()
    {
        // Hide all panels initially and set up their callbacks
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            if (tutorialPanels[i] != null)
            {
                tutorialPanels[i].gameObject.SetActive(false);
                tutorialPanels[i].Initialize(this, i);
            }
        }
    }

    public void StartTutorial()
    {
        if (tutorialPanels.Length == 0)
        {
            Debug.LogWarning("No tutorial panels configured!");
            return;
        }

        isTutorialActive = true;
        currentPanelIndex = 0;

        OnTutorialStart?.Invoke();
        ShowCurrentPanel();
    }

    public void NextPanel()
    {
        if (!isTutorialActive) return;

        // Hide current panel
        HideCurrentPanel();

        // Move to next panel
        currentPanelIndex++;

        if (currentPanelIndex >= tutorialPanels.Length)
        {
            // Tutorial completed
            CompleteTutorial();
        }
        else
        {
            ShowCurrentPanel();
        }
    }

    public void PreviousPanel()
    {
        if (!isTutorialActive || currentPanelIndex <= 0) return;

        HideCurrentPanel();
        currentPanelIndex--;
        ShowCurrentPanel();
    }

    public void SkipTutorial()
    {
        if (isTutorialActive)
        {
            HideCurrentPanel();
        }

        isTutorialActive = false;
        currentPanelIndex = -1;

        OnTutorialSkipped?.Invoke();
    }

    public void CompleteTutorial()
    {
        HideCurrentPanel();

        isTutorialActive = false;
        currentPanelIndex = -1;

        // Mark tutorial as completed in PlayerPrefs
        PlayerPrefs.SetInt("TutorialCompleted", 1);

        OnTutorialComplete?.Invoke();
    }

    public void GoToPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= tutorialPanels.Length) return;

        HideCurrentPanel();
        currentPanelIndex = panelIndex;
        ShowCurrentPanel();
    }

    private void ShowCurrentPanel()
    {
        if (currentPanelIndex < 0 || currentPanelIndex >= tutorialPanels.Length) return;

        currentPanel = tutorialPanels[currentPanelIndex];
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(true);
            currentPanel.OnPanelShow();
        }
    }

    private void HideCurrentPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.OnPanelHide();
            currentPanel.gameObject.SetActive(false);
            currentPanel = null;
        }
    }

    // Method to check if user can proceed (for validation-based tutorials)
    public bool CanProceedToNext()
    {
        if (currentPanel != null)
        {
            return currentPanel.IsStepCompleted();
        }
        return true;
    }

    // Method to notify that current step requirements are met
    public void MarkCurrentStepComplete()
    {
        if (currentPanel != null)
        {
            currentPanel.MarkCompleted();
        }
    }
}

