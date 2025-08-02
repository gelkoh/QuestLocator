public static class BarcodeScannerEventManager
{
    public delegate void StartScanningDelegate(BarcodeScannerType type);
    public static event StartScanningDelegate OnStartScanning;

    public delegate void StopScanningDelegate(BarcodeScannerType type);
    public static event StopScanningDelegate OnStopScanning;

    public enum BarcodeScannerType { NONE, AUTO, MANUAL };

    public static void StartScanning(BarcodeScannerType type)
    {
        OnStartScanning?.Invoke(type);
    }

    public static void StopScanning(BarcodeScannerType type)
    {
        OnStopScanning?.Invoke(type);
    }
}