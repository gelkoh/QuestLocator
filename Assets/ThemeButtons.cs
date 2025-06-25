using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Oculus.Interaction; // Wichtig: Namespace des UIThemeManagers

public class ThemeButtons : MonoBehaviour
{
    [System.Serializable]
    public class ThemeButton
    {
        public Button button;
        public int themeIndex;
        public GameObject activeIndicator; // NEU: Referenz zum Active Indicator GameObject
    }

    [SerializeField] private UIThemeManager _themeManager;
    [SerializeField] private List<ThemeButton> _themeButtons;

    void Start()
    {
        if (_themeManager == null)
        {
            Debug.LogError("[ThemeButtons] UIThemeManager reference is missing!", this);
            return;
        }

        // Listener für jeden Button hinzufügen
        foreach (var themeButton in _themeButtons)
        {
            if (themeButton.button == null)
            {
                Debug.LogWarning($"[ThemeButtons] Button reference missing for theme index {themeButton.themeIndex}!", this);
                continue;
            }

            int indexToApply = themeButton.themeIndex; // Eine lokale Kopie für den Closure
            themeButton.button.onClick.AddListener(() => OnThemeButtonClick(indexToApply));

            // Sicherstellen, dass alle Indikatoren zu Beginn deaktiviert sind
            if (themeButton.activeIndicator != null)
            {
                themeButton.activeIndicator.SetActive(false);
            }
        }

        // Initialen Zustand setzen
        UpdateActiveThemeButton(_themeManager.CurrentThemeIndex);
    }

    // Diese Methode wird aufgerufen, wenn einer der Theme-Buttons geklickt wird
    private void OnThemeButtonClick(int themeIndex)
    {
        // Anwenden des Themes über den UIThemeManager
        _themeManager.ApplyTheme(themeIndex);
        // UI der Buttons aktualisieren
        UpdateActiveThemeButton(themeIndex);
    }

    // Aktualisiert den visuellen Zustand aller Buttons
    private void UpdateActiveThemeButton(int activeIndex)
    {
        foreach (var themeButton in _themeButtons)
        {
            if (themeButton.activeIndicator != null)
            {
                // Aktiviere den Indikator, wenn der Button dem aktiven Theme entspricht, sonst deaktiviere ihn
                bool isActive = (themeButton.themeIndex == activeIndex);
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