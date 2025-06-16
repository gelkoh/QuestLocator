using UnityEngine;

public abstract class BaseTutorialPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] protected bool requiresCompletion = false;
    [SerializeField] protected bool autoAdvance = false;
    [SerializeField] protected float autoAdvanceDelay = 3f;

    protected TutorialStateManager tutorialManager;
    protected int panelIndex;
    protected bool isCompleted = false;

    public virtual void Initialize(TutorialStateManager manager, int index)
    {
        tutorialManager = manager;
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
        tutorialManager?.NextPanel();
    }

    // UI hook methods
    public void OnNextButton()
    {
        if (IsStepCompleted())
        {
            tutorialManager?.NextPanel();
        }
    }

    public void OnPreviousButton()
    {
        tutorialManager?.PreviousPanel();
    }

    public void OnSkipButton()
    {
        tutorialManager?.SkipTutorial();
    }
}
