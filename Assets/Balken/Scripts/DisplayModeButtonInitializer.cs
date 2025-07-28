using UnityEngine;
using UnityEngine.UI;
using static DisplayModeManager;

public class DisplayModeButtonInitializer : MonoBehaviour
{
    public Button button;
    public DisplayMode modeToSet;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (DisplayModeManagerInstance != null)
                    Debug.Log($"[DisplayModeButtonInitializer]: Setting mode to {modeToSet}");
                DisplayModeManagerInstance.SetMode(modeToSet);
            });
        }
    }
}