public interface BarcodeScannerInterface
{
    public void StartScanning(); 
    public void StopScanning();
    public bool IsScanning { get; }
}