public interface IBarcodeScanner
{
    public void StartScanning();
    public void StopScanning();
    public bool IsScanning { get; }
}