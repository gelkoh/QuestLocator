using UnityEngine;
using TMPro;
using Oculus.Interaction;

public class WarningPannelParentScript : MonoBehaviour
{
    [SerializeField] GameObject WarnigPannelPrefab;
    
    private UIThemeManagerLocal themeManager;

    void Start()
    {
        themeManager = GetComponentInParent<UIThemeManagerLocal>();
    }

    public void SetUpWarning(string warningText)
    {
        Debug.LogError("Parent");
        GameObject newPanel = Instantiate(WarnigPannelPrefab, transform);
        newPanel.GetComponent<WarningPannelScript>().SetUpWarning(warningText);
        themeManager.ApplyCurrentTheme();
    }
}
