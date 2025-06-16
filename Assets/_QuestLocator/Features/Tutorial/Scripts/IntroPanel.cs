using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntrolPanel : BaseTutorialPanel
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button skipButton;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnNextButton); // From base TutorialPanel

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButton); // From base TutorialPanel
    }
}
