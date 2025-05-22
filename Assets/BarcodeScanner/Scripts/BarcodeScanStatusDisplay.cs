using UnityEngine;
using static BarcodeScanEventManager;

public class BarcodeScanStatusDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _statusDisplayElements;

    void Awake()
    {
        _statusDisplayElements.SetActive(false);    
    }

    void OnEnable()
    {
        OnStartScanning += SetScanningState;
        OnStopScanning += SetIdleState;
    }

    void OnDisable()
    {
        OnStartScanning -= SetScanningState;
        OnStopScanning -= SetIdleState;
    }

    void SetScanningState(BarcodeScannerType type)
    {
        _statusDisplayElements.SetActive(true);
    }

    private void SetIdleState(BarcodeScannerType type)
    {
        _statusDisplayElements.SetActive(false);
    }
}