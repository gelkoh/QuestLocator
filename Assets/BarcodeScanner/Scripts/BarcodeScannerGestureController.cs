using UnityEngine;
using static BarcodeScanEventManager;

public class BarcodeScannerGestureController : MonoBehaviour
{
    private bool isScannerActive = false; // Internal state of the scanner (on/off)

    void Update()
    {
        // Use GetDown to detect a single press
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            if (isScannerActive)
            {
                // If scanner is currently active, pressing the button stops it
                StopScanning(BarcodeScannerType.AUTO);
                isScannerActive = false;
                Debug.Log("Scanner toggled OFF by button press.");
            }
            else
            {
                // If scanner is currently inactive, pressing the button starts it
                StartScanning(BarcodeScannerType.AUTO);
                isScannerActive = true;
                Debug.Log("Scanner toggled ON by button press.");
            }
        }
    }

    // void OnManualScanGestureDetected()
    // {
        
    // }
}
