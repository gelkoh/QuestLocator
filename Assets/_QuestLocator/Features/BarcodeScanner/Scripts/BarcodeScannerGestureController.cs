using UnityEngine;
using static BarcodeManualScanner;
using static BarcodeAutoScanner;
using static BarcodeScannerEventManager;
using static BarcodeScannerStatusManager;

public class BarcodeScannerGestureController : MonoBehaviour
{
    public static BarcodeScannerGestureController BarcodeScannerGestureControllerInstance { get; private set; }
    public bool isLeftHandManualScanner;
    bool isManualScanActive = false;

    private void Awake()
    {
        if (BarcodeScannerGestureControllerInstance == null)
        {
            BarcodeScannerGestureControllerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }




    // NEW
    public void OnLeftManualScannerGesturePerformed()
    {
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;

        if (!BarcodeScannerStatusManagerInstance.IsScannerActive)
        {
            isLeftHandManualScanner = true;
            isManualScanActive = true;
            BarcodeManualScannerInstance?.StartScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeManualScanner started.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeManualScanner could not be started because BarcodeAutoScanner is already active.");
        }
    }

    public void OnLeftManualScannerGestureEnded()
    {
        if (isManualScanActive == true && isLeftHandManualScanner == false) return;
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;

        if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.MANUAL)
        {
            isManualScanActive = false;
            BarcodeManualScannerInstance?.StopScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeManualScanner stopped.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeManualScanner could not be stopped because it was not active.");
        }
    }

    public void OnRightManualScannerGesturePerformed()
    {
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;

        if (!BarcodeScannerStatusManagerInstance.IsScannerActive)
        {
            isManualScanActive = true;
            isLeftHandManualScanner = false;
            BarcodeManualScannerInstance?.StartScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeManualScanner started.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeManualScanner could not be started because BarcodeAutoScanner is already active.");
        }
    }

    public void OnRightManualScannerGestureEnded()
    {
        if (isManualScanActive == true && isLeftHandManualScanner) return;
        if (!IsBarcodeScannerStatusManagerInstanceAvailable()) return;

        if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.MANUAL)
        {
            isManualScanActive = false;
            BarcodeManualScannerInstance?.StopScanning();
            Debug.Log("BarcodeScannerGestureController: BarcodeManualScanner stopped.");
        }
        else
        {
            Debug.LogWarning("BarcodeScannerGestureController: BarcodeManualScanner could not be stopped because it was not active.");
        }
    }
    // NEW END















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
