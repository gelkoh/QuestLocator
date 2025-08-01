using UnityEngine;
using ZXing;
using System;
using System.Collections.Generic;
using System.Threading;
using PassthroughCameraSamples;
using static BarcodeScannerEventManager;
using static BarcodeScannerStatusManager;
using static BarcodeProcessor;

public class BarcodeAutoScanner : MonoBehaviour, BarcodeScannerInterface
{

    // Singleton
    public static BarcodeAutoScanner BarcodeAutoScannerInstance { get; private set; }

    [Header("Passthrough Camera Texture")]
    [SerializeField] private WebCamTextureManager _webCamTextureManager;

    [Header("Scanner Settings")]
    [SerializeField] private int _scanFrequencyInMilliseconds = 1000;

    [Header("ZXing Options")]
    [SerializeField] private bool _tryHarder = true;
    [SerializeField] private bool _tryInverted = false;
    [SerializeField] private bool _autoRotate = true;
    [SerializeField] private string[] _possibleFormatStrings = new string[] { "EAN_13", "CODE_128" };
    [SerializeField] private int[] _allowedLengths = new int[] { 13 };

    private BarcodeReader _barcodeReader;
    private List<BarcodeFormat> _possibleFormats;

    private Thread _scanThread;
    private CancellationTokenSource _cancellation;
    private AutoResetEvent _frameReady = new AutoResetEvent(false);

    private Color32[] _latestPixels;
    private int _latestWidth;
    private int _latestHeight;
    private readonly object _pixelLock = new object();

    private float _now;
    private bool _waitingForProcessorResponse = false;
    private string _lastProcessedBarcode = string.Empty;
    private float _lastProcessedTime;
    private float _rescanCooldown = 3f;

    public bool IsScanning { get; private set; } = false;

    void Awake()
    {
        if (_webCamTextureManager == null)
        {
            Debug.LogError("BarcodeAutoScanner: WebCamTextureManager is not assigned in the inspector");
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

        if (BarcodeAutoScannerInstance == null)
        {
            BarcodeAutoScannerInstance = this;
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
            Debug.LogError("BarcodeAutoScanner: BarcodeProcessorInstance not found. Ensure it exists in the scene.");
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
        if (BarcodeScannerStatusManagerInstance.ActiveScannerType == BarcodeScannerType.AUTO)
        {
            if (_webCamTextureManager.WebCamTexture != null && _webCamTextureManager.WebCamTexture.isPlaying)
            {
                _now = Time.realtimeSinceStartup;

                lock (_pixelLock)
                {
                    _latestPixels = _webCamTextureManager.WebCamTexture.GetPixels32();

                    _latestWidth = _webCamTextureManager.WebCamTexture.width;
                    _latestHeight = _webCamTextureManager.WebCamTexture.height;
                }
            }
        }
    }

    public void StartScanning()
    {
        if (_webCamTextureManager.WebCamTexture == null ||
            !_webCamTextureManager.WebCamTexture.isPlaying)
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

        Debug.Log("BarcodeAutoScanner: Scanning started.");
        BarcodeScannerEventManager.StartScanning(BarcodeScannerType.AUTO);
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

        Debug.Log("BarcodeAutoScanner: Scanning stopped.");
        BarcodeScannerEventManager.StopScanning(BarcodeScannerType.AUTO);
    }

    private void ScanLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (!_waitingForProcessorResponse && _latestPixels != null && _latestWidth > 0 && _latestHeight > 0)
                {
                    var result = _barcodeReader.Decode(_latestPixels, _latestWidth, _latestHeight);

                    if (result != null)
                    {
                        if (result.Text != _lastProcessedBarcode || _now >= _lastProcessedTime + _rescanCooldown)
                        {
                            Debug.Log("BarcodeAutoScanner: Barcode found: " + result.Text);

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
                        Debug.Log("BarcodeAutoScanner: No barcode found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"BarcodeAutoScanner: Decode error: {ex.Message}");
            }

            Thread.Sleep(_scanFrequencyInMilliseconds);
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
            Debug.Log("BarcodeAutoScanner: No product, continuing scan...");
        }
    }
}