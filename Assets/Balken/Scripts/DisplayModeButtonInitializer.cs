using UnityEngine;
using UnityEngine.UI;

public class DisplayModeButtonInitializer : MonoBehaviour
{
    public Button button;
    public DisplayMode modeToSet;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(() => {
                if (DisplayModeManager.Instance != null)
                    DisplayModeManager.Instance.SetMode(modeToSet);
            });
        }
    }
}
