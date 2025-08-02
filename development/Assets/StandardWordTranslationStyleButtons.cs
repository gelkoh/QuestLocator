using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandardWordTranslationStyleButtons : MonoBehaviour
{
    [System.Serializable]
    public class WordTranslationStyleButton
    {
        public Button button;
        public int translationStyleIndex;
        public GameObject activeIndicator;
    }

    [SerializeField] private List<WordTranslationStyleButton> _wordTranslationButtons;
    private int _defaultTranslationStyleIndex = 2;

    void Start()
    {
        foreach (var wordTranslationButton in _wordTranslationButtons)
        {
            if (wordTranslationButton.button == null)
            {
                Debug.LogWarning($"[StandardWordTranslationStyleButtons] Button reference missing for theme index {wordTranslationButton.translationStyleIndex}!", this);
                continue;
            }

            int indexToApply = wordTranslationButton.translationStyleIndex;
            wordTranslationButton.button.onClick.AddListener(() => OnWordTranslationButtonClick(indexToApply));

            if (wordTranslationButton.activeIndicator != null)
            {
                wordTranslationButton.activeIndicator.SetActive(false);
            }
        }

        int lastSelectedTranslationStyleIndex = PlayerPrefs.GetInt("TranslationStyleIndex");
        Debug.Log("[StandardWordTranslationStyleButtons]: last selected translation style index is: " + lastSelectedTranslationStyleIndex);

        if (lastSelectedTranslationStyleIndex == 0)
        {
            UpdateActiveWordTranslationStyle(_defaultTranslationStyleIndex);
        }
        else
        {
            UpdateActiveWordTranslationStyle(lastSelectedTranslationStyleIndex);
        }
    }

    private void OnWordTranslationButtonClick(int translationStyleIndex)
    {
        UpdateActiveWordTranslationStyle(translationStyleIndex);
    }

    private void UpdateActiveWordTranslationStyle(int translationStyleIndex)
    {
        foreach (var wordTranslationButton in _wordTranslationButtons)
        {
            if (wordTranslationButton.activeIndicator != null)
            {
                bool isActive = wordTranslationButton.translationStyleIndex == translationStyleIndex;
                wordTranslationButton.activeIndicator.SetActive(isActive);
                Debug.Log($"[StandardWordTranslationStyleButtons] Style {wordTranslationButton.translationStyleIndex}: Indicator set to {(isActive ? "ACTIVE" : "INACTIVE")}.");

                if (isActive)
                {
                    PlayerPrefs.SetInt("TranslationStyleIndex", wordTranslationButton.translationStyleIndex);
                    PlayerPrefs.Save();
                }
            }
            else
            {
                Debug.LogWarning($"[StandardWordTranslationStyleButtons] Active Indicator missing for button '{wordTranslationButton.button?.name ?? "NULL"}' (Style Index {wordTranslationButton.translationStyleIndex}). Cannot set active state.", wordTranslationButton.button);
            }
        }
    }
}