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

// [RequireComponent(typeof(BarcodeService))]
// public class ManualFrameScanner : MonoBehaviour, IBarcodeScanner
// {
//     [SerializeField] private WebCamTextureManager _passthroughCamera;   
//     [SerializeField] private GameObject _scanFrame;
//     [SerializeField] private float _scanInterval = 0.25f;
//     private Coroutine _scanRoutine;

//     private void Start()
//     {
//         Debug.LogError("MANUAL FRAME MANAGER STARTED");   
//     }

//     private void Awake()
//     {
//         if (_scanFrame == null) Debug.LogError("ManualFrameScanner: _scanFrame is not assigned in Inspector!");
//         if (_passthroughCamera == null) Debug.LogError("ManualFrameScanner: _passthroughCamera is not assigned in Inspector!");
//     }

//     public void StartScan()
//     {
//         if (_scanFrame == null)
//         {
//             Debug.LogError("ManualFrameScanner: _scanFrame is null in StartScan!");
//             return;
//         }

//         var extractor = _scanFrame.GetComponent<BarcodeScanFrameExtractor>();
//         if (extractor == null)
//         {
//             Debug.LogError("ManualFrameScanner: BarcodeScanFrameExtractor is missing on _scanFrame!");
//             return;
//         }

//         if (_passthroughCamera == null)
//         {
//             Debug.LogError("ManualFrameScanner: _passthroughCamera is null in StartScan!");
//             return;
//         }

//         Debug.Log("ManualFrameScanner GameObject active: " + gameObject.activeInHierarchy);
//         Debug.Log("ScanFrame active: " + _scanFrame.activeInHierarchy);
//         Debug.Log("ManualFrameScanner enabled: " + enabled);

//         _scanFrame.SetActive(true);
//         _scanRoutine = StartCoroutine(ScanLoop());
//     }

//     public void StopScan()
//     {
//         if (_scanRoutine != null) StopCoroutine(_scanRoutine);
//         _scanFrame.SetActive(false);
//     }

//     private IEnumerator ScanLoop()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(_scanInterval);

//             var texture = _passthroughCamera.WebCamTexture;
//             var extractor = _scanFrame.GetComponent<BarcodeScanFrameExtractor>();
//             var extractedTexture = extractor.GetExtractedTexture(texture);

//             if (extractedTexture == null) continue;

//             var pixels = extractedTexture.GetPixels32();
//             var width = extractedTexture.width;
//             var height = extractedTexture.height;

//             BarcodeService.Instance.ScanTexture(pixels, width, height);


//             // _passthroughCamera.WebCamTexture;

//             // var frame = _frameProvider.CurrentFrame;
//             // var extractor = _scanFrame.GetComponent<BarcodeScanFrameExtractor>();
//             // var sub = extractor.GetSubFrameFromScanFrame(frame);
//             // if (sub.Pixels == null) continue;
//             // BarcodeService.Instance.ScanTexture(sub.Pixels, sub.Width, sub.Height);
//         }
//     }
// }
