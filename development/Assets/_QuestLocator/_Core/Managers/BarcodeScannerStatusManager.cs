using System;
using UnityEngine;
using static BarcodeScannerEventManager;

public class BarcodeScannerStatusManager : MonoBehaviour
{
    public static BarcodeScannerStatusManager BarcodeScannerStatusManagerInstance { get; private set; }
    public bool IsScannerActive { get; private set; } = false;
    public BarcodeScannerType ActiveScannerType { get; private set; } = BarcodeScannerType.NONE;

    public event Action<bool, BarcodeScannerType> OnScannerStatusChanged;

    private void Awake()
    {
        if (BarcodeScannerStatusManagerInstance == null)
        {
            BarcodeScannerStatusManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        OnStartScanning += HandleStartScanning;
        OnStopScanning += HandleStopScanning;
    }

    private void OnDisable()
    {
        OnStartScanning -= HandleStartScanning;
        OnStopScanning -= HandleStopScanning;
    }

    private void HandleStartScanning(BarcodeScannerType type)
    {
        IsScannerActive = true;
        ActiveScannerType = type;
        Debug.Log($"BarcodeScannerStatusManager: Scanner started: {type}");
        OnScannerStatusChanged?.Invoke(IsScannerActive, ActiveScannerType);
    }

    private void HandleStopScanning(BarcodeScannerType type)
    {
        IsScannerActive = false;
        ActiveScannerType = BarcodeScannerType.NONE;
        Debug.Log($"BarcodeScannerStatusManager: Scanner stopped: {type}");
        OnScannerStatusChanged?.Invoke(IsScannerActive, ActiveScannerType);
    }
}