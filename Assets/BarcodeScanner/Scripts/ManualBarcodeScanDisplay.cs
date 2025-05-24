using UnityEngine;
using static BarcodeScanEventManager;

public class ManualBarcodeScanDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _manualBarcodeScanFrameElements;

    void Awake()
    {
        _manualBarcodeScanFrameElements.SetActive(false);
    }

    void OnEnable()
    {
        OnStartScanning += HandleStartScanning;
        OnStopScanning += HandleStopScanning;
    }

    void OnDisable()
    {
        OnStopScanning -= HandleStopScanning;
        OnStartScanning -= HandleStartScanning;
    }

    private void HandleStartScanning(BarcodeScannerType type)
    {
        _manualBarcodeScanFrameElements.SetActive(true);
    }

    public void HandleStopScanning(BarcodeScannerType type)
    {
        _manualBarcodeScanFrameElements.SetActive(false);
    }
}
