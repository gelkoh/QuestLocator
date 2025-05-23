using UnityEngine;
using static BarcodeScanEventManager;

// Dieses Skript ist dafür zuständig, das manuelle Scan-Frame-Prefab zu instanziieren
// und dessen Sichtbarkeit basierend auf den Events zu steuern.
public class GestureFramePlacerHolder : MonoBehaviour
{
    // Diese Referenz muss im Inspector zugewiesen werden!
    // Dies ist das Prefab des manuellen Scan-Frames (das GameObject mit dem GestureFramePlacer-Skript).
    [SerializeField] private GameObject _manualBarcodeScanFramePrefab;

    private GameObject _instantiatedScanFrame; // Referenz zur instanziierten Instanz des Prefabs

    void Awake()
    {
        // Instanziiere das Prefab einmalig
        if (_manualBarcodeScanFramePrefab != null)
        {
            _instantiatedScanFrame = Instantiate(_manualBarcodeScanFramePrefab);
            // Stelle sicher, dass die instanziierte Instanz am Anfang ausgeblendet ist
            _instantiatedScanFrame.SetActive(false);
            Debug.LogWarning("GestureFramePlacerHolder: Manueller Scan-Frame Prefab instanziiert und initial auf false gesetzt.");
        }
        else
        {
            Debug.LogError("GestureFramePlacerHolder: _manualBarcodeScanFramePrefab wurde nicht zugewiesen!");
            enabled = false;
        }
    }

    void OnEnable()
    {
        // Registriere dich für die Events
        OnStartScanning += HandleStartScanning;
        OnStopScanning += HandleStopScanning;
        Debug.LogWarning("GestureFramePlacerHolder: Events registriert.");
    }

    void OnDisable()
    {
        // Deregistriere dich von den Events
        OnStopScanning -= HandleStopScanning;
        OnStartScanning -= HandleStopScanning;
        Debug.LogWarning("GestureFramePlacerHolder: Events deregistriert.");
    }

    private void HandleStartScanning(BarcodeScannerType type)
    {
        // Aktiviere den manuellen Scan-Frame NUR, wenn der Typ MANUAL ist
        if (type == BarcodeScannerType.MANUAL)
        {
            if (_instantiatedScanFrame != null)
            {
                _instantiatedScanFrame.SetActive(true);
                Debug.LogWarning("GestureFramePlacerHolder: Manueller Scan-Frame aktiviert.");
            }
        }
    }
    
    private void HandleStopScanning(BarcodeScannerType type)
    {
        // Deaktiviere den manuellen Scan-Frame NUR, wenn der Typ MANUAL ist
        if (type == BarcodeScannerType.MANUAL)
        {
            if (_instantiatedScanFrame != null)
            {
                _instantiatedScanFrame.SetActive(false);
                Debug.LogWarning("GestureFramePlacerHolder: Manueller Scan-Frame deaktiviert.");
            }
        }
    }

    void OnDestroy()
    {
        // Zerstöre die instanziierte Instanz, wenn der Holder zerstört wird
        if (_instantiatedScanFrame != null)
        {
            Destroy(_instantiatedScanFrame);
        }
    }
}