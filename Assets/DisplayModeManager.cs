using System;
using TMPro;
using UnityEngine;

public enum DisplayMode
{
    Per100gVsNeed,
    PerPortionVsNeed,
    Per100gVsLimit
}

public class DisplayModeManager : MonoBehaviour
{
    public static DisplayModeManager Instance;

    public TMP_Text activeModeText;

    public DisplayMode CurrentMode { get; private set; } = DisplayMode.Per100gVsNeed;

    public event Action<DisplayMode> OnDisplayModeChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        // Optional: Standard-Text setzen, falls schon ein Textfeld zugewiesen ist
        if (activeModeText != null)
        {
            activeModeText.SetText(GetTextForMode(CurrentMode));
        }
    }

    public void SetActiveModeText(TMP_Text text)
    {
        activeModeText = text;
        // Direkt aktuellen Modus reinschreiben
        activeModeText.SetText(GetTextForMode(CurrentMode));
    }

    public void SetMode(DisplayMode newMode)
    {
        CurrentMode = newMode;
        activeModeText?.SetText(GetTextForMode(newMode));
        OnDisplayModeChanged?.Invoke(newMode);
    }

    private string GetTextForMode(DisplayMode mode)
    {
        return mode switch
        {
            DisplayMode.Per100gVsNeed => "100g vs Tagesbedarf",
            DisplayMode.PerPortionVsNeed => "Portion vs Tagesbedarf",
            DisplayMode.Per100gVsLimit => "100g vs empfohlene Grenzwerte",
            _ => "Unbekannter Modus"
        };
    }
}
