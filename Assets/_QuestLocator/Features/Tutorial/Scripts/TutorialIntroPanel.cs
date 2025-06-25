using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialIntrolPanel : BaseTutorialPanel
{
    [Header("UI Elements")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [Header("TTS")]
    [SerializeField] private string TTSText = "";

    // Reference to the TTSSpeaker component in this panel
    [SerializeField] private TTSSpeaker ttsSpeaker;

    private void Start()
    {
        // Auto-assign TTSSpeaker in children, then in the scene
        if (ttsSpeaker == null)
            ttsSpeaker = GetComponentInChildren<TTSSpeaker>();
        if (ttsSpeaker == null)
            ttsSpeaker = Object.FindFirstObjectByType<TTSSpeaker>();

        // Auto-assign nextButton if not set
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

        // Auto-assign skipButton if not set
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

        if (ttsSpeaker != null)
        {
            ttsSpeaker.Speak(TTSText);
        }
        else
        {
            Debug.LogWarning("TTSSpeaker not found on IntroPanel or in the scene. Please add a TTSSpeaker component.");
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButton); // From base TutorialPanel  

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButton); // From base TutorialPanel  
    }
}
