using UnityEngine;
using static BarcodeScanEventManager;

public class BarcodeScanStatusDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _scanStatusDisplayElements;

    private void Awake()
    {
        _scanStatusDisplayElements.SetActive(false);
    }

    private void OnEnable()
    {
        OnStartScanning += HandleScanStarted;
        OnStopScanning += HandleScanStopped;
        BarcodeProcessor.Instance.OnProductProcessed += HandleProductProcessed;
    }

    private void OnDisable()
    {
        OnStartScanning -= HandleScanStarted;
        OnStopScanning -= HandleScanStopped;
        BarcodeProcessor.Instance.OnProductProcessed -= HandleProductProcessed;
    }

    private void HandleScanStarted(BarcodeScannerType type)
    {
        _scanStatusDisplayElements.SetActive(true);
    }

    private void HandleScanStopped(BarcodeScannerType type)
    {
        _scanStatusDisplayElements.SetActive(false);
    }

    private void HandleProductProcessed(bool success, string message, Root root)
    {
        if (success)
        {
            _scanStatusDisplayElements.SetActive(false);
        }
    }
}
