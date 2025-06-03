using UnityEngine;
using static BarcodeManualScanner;
using static BarcodeAutoScanner;
using static BarcodeScannerEventManager;
using static BarcodeScannerStatusManager;

public class BarcodeScannerGestureController : MonoBehaviour
{
    public void OnManualScannerGesturePerformed()
    {
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;
        
        if (!BarcodeScannerStatusManagerInstance.IsScannerActive)
        {
            BarcodeManualScannerInstance?.StartScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeManualScanner started.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeManualScanner could not be started because BarcodeAutoScanner is already active.");
        }
    }

    public void OnManualScannerGestureEnded()
    {
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;

        if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.MANUAL)
        {
            BarcodeManualScannerInstance?.StopScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeManualScanner stopped.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeManualScanner could not be stopped because it was not active.");
        }
    }

    public void OnAutoScannerGesturePerformed()
    {
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;

        if (!BarcodeScannerStatusManagerInstance.IsScannerActive)
        {
            BarcodeAutoScannerInstance?.StartScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeAutoScanner started.");
        }
        else if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.AUTO)
        {
            BarcodeAutoScannerInstance?.StopScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeAutoScanner stopped.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeAutoScanner could not be stopped because it was not active.");
        }
    }

    private bool IsBarcodeScannerStatusManagerInstanceAvailable()
    {
        if (BarcodeScannerStatusManagerInstance == null)
        {
            Debug.LogError("BarcodeScannerGestureController: BarcodeScannerStatusManagerInstance is null.");
            return false;
        }

        return true;
    }
}