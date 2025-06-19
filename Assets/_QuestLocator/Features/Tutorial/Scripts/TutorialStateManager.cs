using UnityEngine;
using UnityEngine.Events;

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
    private PanelPositioner _tutorialPanelPositioner;

    // Singleton for easy access
    public static TutorialStateManager TutorialStateManagerInstance { get; private set; }

    public bool IsTutorialActive => isTutorialActive;
    public int CurrentStep => currentPanelIndex;
    public int TotalSteps => tutorialPanels.Length;
    private bool _panelsInitializedYet = false;

    private void Awake()
    {
        Debug.Log("[TutorialStateManager] Awake called.");
        if (TutorialStateManagerInstance == null)
        {
            TutorialStateManagerInstance = this;

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            
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
        if (!_panelsInitializedYet)
        {
            Debug.Log($"[TutorialStateManager] Initializing {tutorialPanels.Length} panels.");

            if (tutorialPanels[0] != null)
            {
                _tutorialPanelPositioner = tutorialPanels[0].GetComponentInChildren<Canvas>().GetComponent<PanelPositioner>();
            }

            // Hide all panels initially and set up their callbacks
            for (int i = 0; i < tutorialPanels.Length; i++)
            {
                if (tutorialPanels[i] != null)
                {
                    Debug.Log($"[TutorialStateManager] Hiding and initializing panel {i}: {tutorialPanels[i].gameObject.name}");
                    tutorialPanels[i].gameObject.SetActive(false);
                    tutorialPanels[i].Initialize(this, i);
                }
                else
                {
                    Debug.LogWarning($"[TutorialStateManager] tutorialPanels[{i}] is null!");
                }
            }

            _panelsInitializedYet = true;
        }
    }

    public void StartTutorial()
    {
        InitializePanels();

        isTutorialActive = !isTutorialActive;

        // If tutorial is already active and menu button is clicked again, hide the tutorial
        if (isTutorialActive)
        {
            Debug.Log("[TutorialStateManager] StartTutorial called.");

            if (tutorialPanels.Length == 0)
            {
                Debug.LogWarning("[TutorialStateManager] No tutorial panels configured!");
                return;
            }

            isTutorialActive = true;
            currentPanelIndex = 0;

            Debug.Log("[TutorialStateManager] Invoking OnTutorialStart event.");
            OnTutorialStart?.Invoke();
            ShowCurrentPanel();

            Debug.Log("[TutorialStateManager] Positioned tutorial in front of camera.");
            _tutorialPanelPositioner.PositionPanelInFrontOfCamera();
        }
        else
        {
            HideCurrentPanel();
            return;
        }
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
        Debug.Log($"[TutorialStateManager] ShowCurrentPanel called. currentPanelIndex={currentPanelIndex}, totalPanels={tutorialPanels.Length}");

        if (currentPanelIndex < 0 || currentPanelIndex >= tutorialPanels.Length)
        {
            Debug.LogWarning("[TutorialStateManager] currentPanelIndex out of range.");
            return;
        }

        currentPanel = tutorialPanels[currentPanelIndex];

        if (currentPanel != null)
        {
            Debug.Log($"[TutorialStateManager] Showing panel {currentPanelIndex}: {currentPanel.gameObject.name}");
            currentPanel.gameObject.SetActive(true);
            currentPanel.OnPanelShow();
        }
        else
        {
            Debug.LogWarning($"[TutorialStateManager] currentPanel at index {currentPanelIndex} is null!");
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

    public void ResetAndHideTutorial()
    {
        Debug.Log("[TutorialStateManager] ResetAndHideTutorial called by external source.");
        if (isTutorialActive)
        {
            HideCurrentPanel();
        }

        foreach (var panel in tutorialPanels)
        {
            if (panel != null && panel.gameObject.activeSelf)
            {
                panel.gameObject.SetActive(false);
            }
        }

        isTutorialActive = false;
        currentPanelIndex = -1;
    }
}
