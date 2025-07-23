using UnityEngine;
using ZXing;
using System;
using System.Collections.Generic;
using System.Threading;
using PassthroughCameraSamples;
using static BarcodeScannerEventManager;
using static BarcodeScannerStatusManager;
using static BarcodeProcessor;

public class BarcodeManualScanner : MonoBehaviour, BarcodeScannerInterface
{
    // Singleton
    public static BarcodeManualScanner BarcodeManualScannerInstance { get; private set; }

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private WebCamTextureManager _webCamTextureManager;
    [SerializeField] private Renderer _extractedTextureDisplay;
    // [SerializeField] private BarcodeManualScannerScanFrameExtractor _barcodeManualScannerScanFrameExtractor;
    // [SerializeField] private BarcodeScanCameraTracker _barcodeScanCameraTracker;
    [SerializeField] private Renderer _webCamDisplayQuad;
    [SerializeField] private float _scanFrequency = 1f;


    [Header("ZXing Options")]
    [SerializeField] private bool _tryHarder = false;
    [SerializeField] private bool _tryInverted = false;
    [SerializeField] private bool _autoRotate = false;
    [SerializeField] private bool _pureBarcode = true;
    // Add tooltip for possible strings
    [SerializeField] private string[] _possibleFormatStrings = new string[] { "EAN_13", "CODE_128" };
    [SerializeField] private int[] _allowedLengths = new int[] { 13 };

    private BarcodeReader _barcodeReader;
    private List<BarcodeFormat> _possibleFormats;

    private Thread _scanThread;
    private CancellationTokenSource _cancellation;
    private AutoResetEvent _frameReady = new(false);

    private Color32[] _latestExtractedPixels;
    private int _latestExtractedWidth;
    private int _latestExtractedHeight;
    private readonly object _pixelLock = new();

    private float _now;
    private float _lastExtractionTime = 0f;
    private float _extractionFrequency = 0.5f;
    private bool _waitingForProcessorResponse = false;
    private string _lastProcessedBarcode = string.Empty;
    private float _lastProcessedTime;
    private float _rescanCooldown = 3f;

    public bool IsScanning { get; private set; } = false;


    [SerializeField] private ExtractionCameraController _extractionCameraController;
    [SerializeField] private RectTransform _scanFrameRect;
    [SerializeField] private HandRelativePositionCalculator handPositionCalculator;

    void Awake()
    {
        if (_webCamTextureManager == null)
        {
            Debug.LogError("BarcodeManualScanner: WebCamTextureManager is not assigned in the inspector");
            return;
        }

        _possibleFormats = new List<BarcodeFormat>();

        foreach (var formatString in _possibleFormatStrings)
        {
            if (Enum.TryParse(formatString, out BarcodeFormat format))
            {
                _possibleFormats.Add(format);
            }
            else
            {
                Debug.LogError($"Invalid barcode format string: {formatString}");
            }
        }

        try
        {
            _barcodeReader = new BarcodeReader
            {
                AutoRotate = _autoRotate, // Rotate the barcode 3 times by 90 degrees to scan from all sides
                Options = new ZXing.Common.DecodingOptions
                {
                    // Use more processing power to increase precision
                    TryHarder = _tryHarder,
                    TryInverted = _tryInverted,
                    PureBarcode = _pureBarcode,
                    // Only allow formats used for product EANs to increase processing speed
                    PossibleFormats = _possibleFormats,
                    AllowedLengths = _allowedLengths
                }
            };
        }
        catch (Exception ex)
        {
            Debug.LogError("BarcodeAutoScanner: Barcode reader could not be initialized: " + ex.Message);
        }

        if (BarcodeManualScannerInstance == null)
        {
            BarcodeManualScannerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (BarcodeProcessorInstance != null)
        {
            BarcodeProcessorInstance.OnProductProcessed += HandleProductProcessed;
        }
        else
        {
            Debug.LogError("BarcodeManualScanner: BarcodeProcessorInstance not found. Ensure it exists in the scene.");
            return;
        }
    }

    void OnDisable()
    {
        if (BarcodeProcessorInstance != null)
        {
            BarcodeProcessorInstance.OnProductProcessed -= HandleProductProcessed;
        }

        StopScanning();
    }

    void Update()
    {
        if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.MANUAL)
        {
            if (_webCamTextureManager.WebCamTexture != null && _webCamTextureManager.WebCamTexture.isPlaying)
            {
                _now = Time.realtimeSinceStartup;

                if (_now >= _lastExtractionTime + _extractionFrequency)
                {
                    if (_webCamDisplayQuad != null && _mainCamera != null)
                    {
                        float webcamWidth = _webCamTextureManager.WebCamTexture.width;
                        float webcamHeight = _webCamTextureManager.WebCamTexture.height;
                        float webcamAspectRatio = webcamWidth / webcamHeight;

                        float quadWorldZDistance = 1f; 

                        float mainCamFOV_rad = _mainCamera.fieldOfView * Mathf.Deg2Rad;
                        float quadHeightInWorld = 2f * quadWorldZDistance * Mathf.Tan(mainCamFOV_rad / 2f);

                        float quadWidthInWorld = quadHeightInWorld * webcamAspectRatio;

                        _webCamDisplayQuad.transform.localScale = new Vector3(quadWidthInWorld, quadHeightInWorld, 1f);

                        _webCamDisplayQuad.transform.rotation = _mainCamera.transform.rotation;
                        // _webCamDisplayQuad.transform.position = _mainCamera.transform.position + _mainCamera.transform.forward * quadWorldZDistance;

            

                        Debug.Log($"[BarcodeManualScanner] Quad Setup: Scale={_webCamDisplayQuad.transform.localScale}, Pos={_webCamDisplayQuad.transform.position}, Rot={_webCamDisplayQuad.transform.rotation}");
                    }

                    _webCamDisplayQuad.material.mainTexture = _webCamTextureManager.WebCamTexture;

                    var latestExtractedTexture = _extractionCameraController.ExtractPixels();

                    if (latestExtractedTexture == null)
                    {
                        Debug.LogWarning("[BarcodeManualScanner] ExtractPixels returned null texture");
                        return;
                    }

                    Debug.Log($"[BarcodeManualScanner] Extracted texture dimensions: {latestExtractedTexture.width}x{latestExtractedTexture.height}");

                    lock (_pixelLock)
                    {
                        _latestExtractedPixels = latestExtractedTexture.GetPixels32();
                        _latestExtractedWidth = latestExtractedTexture.width;
                        _latestExtractedHeight = latestExtractedTexture.height;
                    }

                    if (_latestExtractedPixels != null && _latestExtractedPixels.Length > 0 && _latestExtractedWidth > 0 && _latestExtractedHeight > 0)
                    {
                        _extractedTextureDisplay.material.mainTexture = latestExtractedTexture;
                        Debug.Log($"[BarcodeManualScanner] Successfully updated texture display with {_latestExtractedPixels.Length} pixels");
                    }
                    else
                    {
                        Debug.LogWarning($"[BarcodeManualScanner] Extracted pixels problem - Pixels: {(_latestExtractedPixels?.Length ?? 0)}, Width: {_latestExtractedWidth}, Height: {_latestExtractedHeight}");
                    }

                    // Don't forget to clean up the texture if you're not using it elsewhere
                    if (latestExtractedTexture != _extractedTextureDisplay.material.mainTexture)
                    {
                        DestroyImmediate(latestExtractedTexture);
                    }

                    _lastExtractionTime = _now;
                }
            }
        }
    }

    public void StartScanning()
    {
        if (_webCamTextureManager.WebCamTexture == null || !_webCamTextureManager.WebCamTexture.isPlaying)
        {
            Debug.LogWarning("Cannot start scanning, camera not ready.");
            return;
        }

        if (IsScanning)
            return;

        IsScanning = true;
        _lastProcessedTime = -Mathf.Infinity;

        // Start background thread
        _cancellation = new CancellationTokenSource();

        _scanThread = new Thread(() => ScanLoop(_cancellation.Token))
        {
            IsBackground = true
        };

        _scanThread.Start();

        Debug.Log("BarcodeManualScanner: Scanning started.");
        BarcodeScannerEventManager.StartScanning(BarcodeScannerType.MANUAL);
    }

    public void StopScanning()
    {
        if (!IsScanning)
            return;

        IsScanning = false;
        _cancellation?.Cancel();
        _frameReady.Set();
        _scanThread = null;
        _cancellation = null;

        Debug.Log("BarcodeManualScanner: Scanning stopped.");
        BarcodeScannerEventManager.StopScanning(BarcodeScannerType.MANUAL);
    }

    private void ScanLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (!_waitingForProcessorResponse && _latestExtractedPixels != null && _latestExtractedWidth > 0 && _latestExtractedHeight > 0)
                {
                    var result = _barcodeReader.Decode(_latestExtractedPixels, _latestExtractedWidth, _latestExtractedHeight);

                    if (result != null)
                    {
                        if (result.Text != _lastProcessedBarcode || _now >= _lastProcessedTime + _rescanCooldown)
                        {
                            Debug.LogWarning("BarcodeManualScanner: Barcode found: " + result.Text);

                            _lastProcessedBarcode = result.Text;
                            _lastProcessedTime = _now;
                            _waitingForProcessorResponse = true;

                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                BarcodeProcessorInstance.ProcessBarcode(result.Text);
                            });
                        }
                    }
                    else
                    {
                        Debug.LogWarning("BarcodeManualScanner: No barcode found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"BarcodeManualScanner: Decode error: {ex.Message}");
            }

            Thread.Sleep((int)(_scanFrequency * 1000));
        }
    }

    private void HandleProductProcessed(bool success, string productNameOrError, Root product)
    {
        _waitingForProcessorResponse = false;

        if (success)
        {
            UnityMainThreadDispatcher.Enqueue(StopScanning);
        }
        else
        {
            Debug.Log("BarcodeManualScanner: No product, continuing scan...");
        }
    }
}