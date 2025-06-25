using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSettingsPanel : BaseTutorialPanel
{
    [Header("UI Elements")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [Header("TTS")]
    [SerializeField] private string TTSText = "";

    // Reference to the TTSSpeaker component in this panel
    [SerializeField] private TTSSpeaker ttsSpeaker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

        // Auto-assign previousButton if not set
        if (previousButton == null)
        {
            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                if (btn.name.ToLower().Contains("previous"))
                {
                    previousButton = btn;
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
            Debug.LogWarning("TTSSpeaker not found on Panel. Please add a TTSSpeaker component as a child.");
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButton); // From base TutorialPanel  

        if (previousButton != null)
            previousButton.onClick.AddListener(OnPreviousButton); // From base TutorialPanel 
    }
}
