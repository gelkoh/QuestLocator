using UnityEngine;
using static BarcodeScanEventManager;

public class BarcodeScannerGestureController : MonoBehaviour
{
    private bool isScannerActive = false; // Interner Zustand des Scanners (an/aus)

    // Wichtig: Diese Methode muss aufgerufen werden, wenn der AUTO-Scanner stoppt,
    // z.B. wenn ein Barcode erfolgreich verarbeitet wurde.
    private void OnEnable()
    {
        OnStopScanning += HandleScannerStopped;
    }

    private void OnDisable()
    {
        OnStopScanning -= HandleScannerStopped;
    }

    private void HandleScannerStopped(BarcodeScannerType type)
    {
        // Setze den isScannerActive-Zustand nur zurück, wenn es der AUTO-Scanner war,
        // der gestoppt wurde.
        if (type == BarcodeScannerType.AUTO)
        {
            isScannerActive = false;
            Debug.Log("BarcodeScannerGestureController: Scanner-Zustand für AUTO auf INAKTIV zurückgesetzt.");
        }
    }

    void Update()
    {
        // Logik für den AUTO-Scanner (Toggelt bei Button 4)
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            if (isScannerActive)
            {
                // Wenn Scanner aktiv, stoppe ihn
                StopScanning(BarcodeScannerType.AUTO);
                // isScannerActive wird durch HandleScannerStopped zurückgesetzt
                Debug.Log("BarcodeScannerGestureController: AUTO-Scanner-Toggle OFF.");
            }
            else
            {
                // Wenn Scanner inaktiv, starte ihn
                StartScanning(BarcodeScannerType.AUTO);
                isScannerActive = true; // Setze sofort auf aktiv
                Debug.Log("BarcodeScannerGestureController: AUTO-Scanner-Toggle ON.");
            }
        }
    }

    // Diese Methoden müssen von deinem Gestenerkennungssystem aufgerufen werden.
    // Z.B. über UnityEvents im Inspector oder durch ein anderes Skript.
    public void OnHandleManualScanGesturePerformed()
    {
        // Debug.LogWarning("Inside OnHandleManualScanGesturePerformed");

        if (!isScannerActive) // Nur starten, wenn nicht bereits ein Scanner aktiv ist
        {
            StartScanning(BarcodeScannerType.MANUAL);
            isScannerActive = true;
            Debug.LogWarning("BarcodeScannerGestureController: Manueller Scanner gestartet.");
        }
    }

    public void OnHandleManualScanGestureEnded()
    {
        // Debug.LogWarning("Inside OnHandleManualScanGestureEnded");

        if (isScannerActive) // Nur stoppen, wenn ein Scanner aktiv ist
        {
            StopScanning(BarcodeScannerType.MANUAL);
            isScannerActive = false;
            Debug.LogWarning("BarcodeScannerGestureController: Manueller Scanner gestoppt.");
        }
    }
}

// using UnityEngine;
// using static BarcodeScanEventManager;

// public class BarcodeScannerGestureController : MonoBehaviour
// {
//     private bool isScannerActive = false; // Internal state of the scanner (on/off)

//     private void OnEnable()
//     {
//         OnStopScanning += HandleScannerStopped;
//     }

//     private void OnDisable()
//     {
//         OnStopScanning -= HandleScannerStopped;
//     }

//     private void HandleScannerStopped(BarcodeScannerType type)
//     {
//         isScannerActive = false;
//         Debug.Log("Scanner state reset to INACTIVE from external StopScanning call.");
//     }

//     void Update()
//     {
//         // Use GetDown to detect a single press
//         if (OVRInput.GetDown(OVRInput.Button.Four))
//         {
//             if (isScannerActive)
//             {
//                 // If scanner is currently active, pressing the button stops it
//                 StopScanning(BarcodeScannerType.AUTO);
//                 isScannerActive = false;
//                 Debug.Log("Scanner toggled OFF by button press.");
//             }
//             else
//             {
//                 // If scanner is currently inactive, pressing the button starts it
//                 StartScanning(BarcodeScannerType.AUTO);
//                 isScannerActive = true;
//                 Debug.Log("Scanner toggled ON by button press.");
//             }
//         }
//     }

//     // public void HandleAutoScanButtonPressed()
//     // {
//     //     if (isScannerActive)
//     //     {
//     //         // If scanner is currently active, pressing the button stops it
//     //         StopScanning(BarcodeScannerType.AUTO);
//     //         isScannerActive = false;
//     //         Debug.LogWarning("Scanner toggled OFF by button press.");
//     //     }
//     //     else
//     //     {
//     //         // If scanner is currently inactive, pressing the button starts it
//     //         StartScanning(BarcodeScannerType.AUTO);
//     //         isScannerActive = true;
//     //         Debug.LogWarning("Scanner toggled ON by button press.");
//     //     }
//     // }

//     public void OnHandleManualScanGesturePerformed()
//     {
//         if (!isScannerActive)
//         {
//             // If scanner is currently inactive, pressing the button starts it
//             StartScanning(BarcodeScannerType.MANUAL);
//             isScannerActive = true;
//             Debug.LogWarning("Manual scanner started");
//         }
//     }

//     public void OnHandleManualScanGestureEnded()
//     {
//         if (isScannerActive)
//         {
//             // If scanner is currently active, pressing the button stops it
//             StopScanning(BarcodeScannerType.MANUAL);
//             isScannerActive = false;
//             Debug.LogWarning("Manual scanner stopped");
//         }
//     }
// }
