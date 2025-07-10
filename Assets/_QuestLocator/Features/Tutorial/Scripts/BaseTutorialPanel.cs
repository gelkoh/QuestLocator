using UnityEngine;

public abstract class BaseTutorialPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] protected bool requiresCompletion = false;
    [SerializeField] protected bool autoAdvance = false;
    [SerializeField] protected float autoAdvanceDelay = 3f;

    protected TutorialStateManager tutorialStateManager;
    protected int panelIndex;
    protected bool isCompleted = false;

    public virtual void Initialize(TutorialStateManager manager, int index)
    {
        tutorialStateManager = manager;
        panelIndex = index;
        isCompleted = false;
    }

    public virtual void OnPanelShow()
    {
        if (autoAdvance && !requiresCompletion)
        {
            Invoke(nameof(AutoAdvance), autoAdvanceDelay);
        }
    }

    public virtual void OnPanelHide()
    {
        CancelInvoke(nameof(AutoAdvance));
    }

    public virtual bool IsStepCompleted()
    {
        return !requiresCompletion || isCompleted;
    }

    public virtual void MarkCompleted()
    {
        isCompleted = true;
        if (autoAdvance)
        {
            Invoke(nameof(AutoAdvance), autoAdvanceDelay);
        }
    }

    private void AutoAdvance()
    {
        tutorialStateManager?.NextPanel();
    }

    // UI hook methods
    public void OnNextButton()
    {
        if (IsStepCompleted())
        {
            tutorialStateManager?.NextPanel();
        }
    }

    public void OnPreviousButton()
    {
        tutorialStateManager?.PreviousPanel();
    }

    public void OnSkipButton()
    {
        tutorialStateManager?.SkipTutorial();
    }
}
