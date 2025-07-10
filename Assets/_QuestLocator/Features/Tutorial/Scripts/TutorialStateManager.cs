using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TutorialStateManager : MonoBehaviour
{
    [Header("Tutorial Panels")]
    [SerializeField] private BaseTutorialPanel[] tutorialPanelPrefabs; // Changed from tutorialPanels to tutorialPanelPrefabs
    [SerializeField] private Transform contentRoot; // Parent transform where panels should be spawned

    [Header("Events")]
    public UnityEvent OnTutorialStart;
    public UnityEvent OnTutorialComplete;
    public UnityEvent OnTutorialSkipped;

    private int currentPanelIndex = -1;
    private bool isTutorialActive = false;
    private BaseTutorialPanel currentPanel;
    private PanelPositioner _currentPanelPositioner;
    private bool firstPanelWasPositioned = false;
    private Vector3 firstPanelPosition;
    private Quaternion firstPanelRotation;

    // Singleton for easy access
    public static TutorialStateManager TutorialStateManagerInstance { get; private set; }

    public bool IsTutorialActive => isTutorialActive;
    public int CurrentStep => currentPanelIndex;
    public int TotalSteps => tutorialPanelPrefabs.Length;

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

        StartCoroutine(UpdatePanelPositionLoop());
    }

    private IEnumerator UpdatePanelPositionLoop()
    {
        while (true)
        {
            if (currentPanel != null)
            {
                Transform canvasRootTransform = currentPanel.transform.Find("CanvasRoot");

                if (canvasRootTransform != null)
                {
                    firstPanelPosition = canvasRootTransform.position;
                    firstPanelRotation = canvasRootTransform.rotation;
                }
                else
                {
                    Debug.LogWarning("[TutorialStateManager]: CanvasRoot not found on currentPanel. Cannot update firstPanelPosition/Rotation.");
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    public void StartTutorial()
    {
        isTutorialActive = !isTutorialActive;

        // If tutorial is already active and menu button is clicked again, hide the tutorial
        if (isTutorialActive)
        {
            Debug.Log("[TutorialStateManager] StartTutorial called.");

            if (tutorialPanelPrefabs.Length == 0)
            {
                Debug.LogWarning("[TutorialStateManager] No tutorial panel prefabs configured!");
                return;
            }

            if (contentRoot == null)
            {
                Debug.LogError("[TutorialStateManager] Content root is not assigned! Please assign a content root transform.");
                return;
            }

            isTutorialActive = true;
            currentPanelIndex = 0;

            Debug.Log("[TutorialStateManager] Invoking OnTutorialStart event.");
            OnTutorialStart?.Invoke();
            ShowCurrentPanel();
        }
        else
        {
            SkipTutorial();
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

        if (currentPanelIndex >= tutorialPanelPrefabs.Length)
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
        firstPanelWasPositioned = false;

        HideCurrentPanel();

        isTutorialActive = false;
        currentPanelIndex = -1;

        OnTutorialSkipped?.Invoke();
    }

    public void CompleteTutorial()
    {
        firstPanelWasPositioned = false;

        HideCurrentPanel();

        isTutorialActive = false;
        currentPanelIndex = -1;

        // Mark tutorial as completed in PlayerPrefs
        PlayerPrefs.SetInt("TutorialCompleted", 1);

        OnTutorialComplete?.Invoke();
    }

    public void GoToPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= tutorialPanelPrefabs.Length) return;

        HideCurrentPanel();
        currentPanelIndex = panelIndex;
        ShowCurrentPanel();
    }

    private void ShowCurrentPanel()
    {
        Debug.Log($"[TutorialStateManager] ShowCurrentPanel called. currentPanelIndex={currentPanelIndex}, totalPanels={tutorialPanelPrefabs.Length}");

        if (currentPanelIndex < 0 || currentPanelIndex >= tutorialPanelPrefabs.Length)
        {
            Debug.LogWarning("[TutorialStateManager] currentPanelIndex out of range.");
            return;
        }

        BaseTutorialPanel prefab = tutorialPanelPrefabs[currentPanelIndex];

        if (prefab != null)
        {
            Debug.Log($"[TutorialStateManager] Instantiating panel {currentPanelIndex}: {prefab.gameObject.name}");

            // Instantiate the prefab as a child of the content root
            GameObject panelInstance = Instantiate(prefab.gameObject, contentRoot);
            currentPanel = panelInstance.GetComponent<BaseTutorialPanel>();

            // Initialize the panel
            currentPanel.Initialize(this, currentPanelIndex);

            if (!firstPanelWasPositioned)
            {
                // Get the PanelPositioner from the Canvas
                _currentPanelPositioner = panelInstance.GetComponentInChildren<Canvas>().GetComponent<PanelPositioner>();

                // Position and show the panel
                if (_currentPanelPositioner != null)
                {
                    _currentPanelPositioner.PositionPanelInFrontOfCamera();
                }

                // panelInstance.transform.Find("CanvasRoot");
                firstPanelPosition = currentPanel.transform.Find("CanvasRoot").transform.position;
                firstPanelRotation = currentPanel.transform.Find("CanvasRoot").transform.rotation;
                firstPanelWasPositioned = true;
            }
            else
            {
                currentPanel.transform.Find("CanvasRoot").transform.position = firstPanelPosition;
                currentPanel.transform.Find("CanvasRoot").transform.rotation = firstPanelRotation;
                // currentPanel.gameObject.transform.position = firstPanelPosition;currentPanel.transform.Find("CanvasRoot").transform.position;
            }

            Debug.Log($"[TutorialStateManager]: firstPanelWasPosition: {firstPanelWasPositioned}");
            Debug.Log($"[TutorialStateManager]: firstPanelPosition: {firstPanelPosition}");

            currentPanel.OnPanelShow();
        }
        else
        {
            Debug.LogWarning($"[TutorialStateManager] prefab at index {currentPanelIndex} is null!");
        }
    }

    private void HideCurrentPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.OnPanelHide();

            // Destroy the panel instead of deactivating it
            Debug.Log($"[TutorialStateManager] Destroying panel: {currentPanel.gameObject.name}");
            Destroy(currentPanel.gameObject);
            currentPanel = null;
            _currentPanelPositioner = null;
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
        firstPanelWasPositioned = false;

        Debug.Log("[TutorialStateManager] ResetAndHideTutorial called by external source.");

        HideCurrentPanel();

        isTutorialActive = false;
        currentPanelIndex = -1;
    }
}