using UnityEngine;
using TMPro;

public class WarningPannelScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextField;
    private PanelPositioner WarningPanelPositioner;

    public void SetUpWarning(string warningText)
    {
        Debug.LogError("panel");
        TextField.text = warningText;
        WarningPanelPositioner = GetComponent<PanelPositioner>();
        WarningPanelPositioner.PositionPanelInFrontOfCamera();
    }
}