using PassthroughCameraSamples;
using UnityEngine;

public class ManualBarcodeScanner : MonoBehaviour, IBarcodeScanner
{
    [SerializeField] private WebCamTextureManager _webCamTextureManager;

    public bool IsScanning { get; }
    
    public void StartScanning()
    {

    }

    public void StopScanning()
    {

    }

    public void ProcessTexture(Texture2D texture)
    {

    }
}