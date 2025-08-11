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
    public static DisplayModeManager DisplayModeManagerInstance { get; private set; }

    public DisplayMode CurrentMode { get; private set; } = DisplayMode.Per100gVsNeed;

    public event Action<DisplayMode> OnDisplayModeChanged;

    void Awake()
    {
        if (DisplayModeManagerInstance != null && DisplayModeManagerInstance != this)
        {
            Destroy(this);
            return;
        }

        DisplayModeManagerInstance = this;
    }
    public void SetMode(DisplayMode newMode)
    {
        CurrentMode = newMode;
        OnDisplayModeChanged?.Invoke(newMode);
    }

    public string GetTextForMode(DisplayMode mode)
    {
        return mode switch
        {
            DisplayMode.Per100gVsNeed => "100g vs Daily Need",
            DisplayMode.PerPortionVsNeed => "Portion vs Daily Need",
            DisplayMode.Per100gVsLimit => "100g vs Recommended Limit",
            _ => "Unknown Mode"
        };
    }
}
