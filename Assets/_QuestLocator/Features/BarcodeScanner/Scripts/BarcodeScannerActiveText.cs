using UnityEngine;
using static BarcodeScannerStatusManager;
using static BarcodeScannerEventManager;
using TMPro;

public class BarcodeScannerActiveText : MonoBehaviour
{
    private TextMeshProUGUI _barcodeScannerActiveIndicatorText;

    void Awake()
    {
        _barcodeScannerActiveIndicatorText = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        if (BarcodeScannerStatusManagerInstance != null)
        {
            if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.AUTO)
            {
                _barcodeScannerActiveIndicatorText.SetText("Auto Scanner Active");
            }
            else if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.MANUAL)
            {
                _barcodeScannerActiveIndicatorText.SetText("Manual Scanner Active");
            }
        }
    }
}