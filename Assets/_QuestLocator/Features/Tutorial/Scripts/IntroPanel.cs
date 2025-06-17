using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntrolPanel : BaseTutorialPanel
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button skipButton;
    [Header("TTS")]
    [SerializeField] private string TTSText = "Welcome to the tutorial!";

    // Reference to the TTSSpeaker component in this panel
    [SerializeField] private TTSSpeaker ttsSpeaker;

    private void Start()
    {
        // If not assigned in the Inspector, try to find a TTSSpeaker in this GameObject or its children
        if (ttsSpeaker == null)
            ttsSpeaker = GetComponentInChildren<TTSSpeaker>();

        if (ttsSpeaker != null)
        {
            ttsSpeaker.Speak(TTSText);
        }
        else
        {
            Debug.LogWarning("TTSSpeaker not found on IntroPanel. Please add a TTSSpeaker component as a child.");
        }

        if (startButton != null)
            startButton.onClick.AddListener(OnNextButton); // From base TutorialPanel  

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButton); // From base TutorialPanel  
    }
}
