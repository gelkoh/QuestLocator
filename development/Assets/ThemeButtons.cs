using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Oculus.Interaction;

public class ThemeButtons : MonoBehaviour
{
    [System.Serializable]
    public class ThemeButton
    {
        public Button button;
        public int themeIndex;
        public GameObject activeIndicator;
    }

    [SerializeField] private UIThemeManagerLocal _themeManager;
    [SerializeField] private List<ThemeButton> _themeButtons;

    void Start()
    {
        if (_themeManager == null)
        {
            Debug.LogError("[ThemeButtons] UIThemeManager reference is missing!", this);
            return;
        }

        foreach (var themeButton in _themeButtons)
        {
            if (themeButton.button == null)
            {
                Debug.LogWarning($"[ThemeButtons] Button reference missing for theme index {themeButton.themeIndex}!", this);
                continue;
            }

            int indexToApply = themeButton.themeIndex;
            themeButton.button.onClick.AddListener(() => OnThemeButtonClick(indexToApply));

            if (themeButton.activeIndicator != null)
            {
                themeButton.activeIndicator.SetActive(false);
            }
        }

        UpdateActiveThemeButton(_themeManager.CurrentThemeIndex);
    }

    private void OnThemeButtonClick(int themeIndex)
    {
        _themeManager.ApplyTheme(themeIndex);
        UpdateActiveThemeButton(themeIndex);
    }

    private void UpdateActiveThemeButton(int activeIndex)
    {
        foreach (var themeButton in _themeButtons)
        {
            if (themeButton.activeIndicator != null)
            {
                bool isActive = themeButton.themeIndex == activeIndex;
                themeButton.activeIndicator.SetActive(isActive);
                Debug.Log($"[ThemeButtons] Theme {themeButton.themeIndex}: Indicator set to {(isActive ? "ACTIVE" : "INACTIVE")}.");
            }
            else
            {
                Debug.LogWarning($"[ThemeButtons] Active Indicator missing for button '{themeButton.button?.name ?? "NULL"}' (Theme Index {themeButton.themeIndex}). Cannot set active state.", themeButton.button);
            }
        }
    }
}