using System;
using System.Collections;
using System.Collections.Generic;
using PassthroughCameraSamples;
using UnityEngine;
using ZXing;
using static BarcodeScanEventManager;

public class AutoBarcodeScanner : MonoBehaviour, IBarcodeScanner
{
    [SerializeField] private WebCamTextureManager _webCamTextureManager;
    private BarcodeReader _barcodeReader;
    private Coroutine _scanCoroutine;

    public bool IsScanning { get; private set; }
    private string _lastProcessedBarcode = "";
    private float _lastProcessedTime = -Mathf.Infinity;
    [SerializeField] private float _scanFrequency = 0.25f;
    private float _rescanCooldown = 3.0f;
    private bool _waitingForProcessorResponse = false;

    void OnEnable()
    {
        OnStartScanning += HandleStartScanning;
        OnStopScanning += HandleStopScanning;

        if (BarcodeProcessor.Instance != null)
        {
            BarcodeProcessor.Instance.OnProductProcessed += HandleProductProcessed;
        }
        else
        {
            Debug.LogWarning("BarcodeProcessor Instance not found. Ensure it exists in the scene.");
        }
    }

    void OnDisable()
    {
        OnStopScanning -= HandleStopScanning;
        OnStartScanning -= HandleStartScanning;

        if (BarcodeProcessor.Instance != null)
        {
            BarcodeProcessor.Instance.OnProductProcessed -= HandleProductProcessed;
        }
    }

    private IEnumerator Start()
    {
        try
        {
            _barcodeReader = new BarcodeReader
            {
                AutoRotate = true, // Rotate the barcode 3 times by 90 degrees to scan from all rotations
                Options = new ZXing.Common.DecodingOptions
                {
                    // Use more processing power to increase precision
                    TryHarder = true,
                    // Only allow formats used for product EANs to increase processing speed
                    PossibleFormats = new List<BarcodeFormat> {
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.CODE_128
                    }
                }
            };
        }
        catch (Exception ex)
        {
            Debug.LogError("Barcode reader could not be initialized: " + ex.Message);
        }

        if (_webCamTextureManager == null)
        {
            Debug.LogError("WebCamTextureManager is not assigned in the inspector!");
            yield break;
        }

        while (_webCamTextureManager.WebCamTexture == null || !_webCamTextureManager.WebCamTexture.isPlaying)
        {
            Debug.Log("Waiting for WebCamTexture to be ready and playing...");
            yield return null;
        }

        Debug.Log("WebCamTexture is ready. Scanner initialized.");
    }

    void OnDestroy()
    {
        if (_scanCoroutine != null)
        {
            StopCoroutine(_scanCoroutine);
            _scanCoroutine = null;
        }
    }

    private IEnumerator ScanLoop()
    {
        while (true)
        {
            try
            {
                if (!_waitingForProcessorResponse && IsScanning && _webCamTextureManager.WebCamTexture != null && _webCamTextureManager.WebCamTexture.isPlaying)
                {
                    Color32[] pixels = _webCamTextureManager.WebCamTexture.GetPixels32();
                    int width = _webCamTextureManager.WebCamTexture.width;
                    int height = _webCamTextureManager.WebCamTexture.height;

                    if (pixels != null && pixels.Length > 0 && width > 0 && height > 0)
                    {
                        var result = _barcodeReader.Decode(pixels, width, height);

                        if (result != null)
                        {
                            // Cooldown check for scanner-side duplicates
                            if (result.Text == _lastProcessedBarcode && Time.time < _lastProcessedTime + _rescanCooldown)
                            {
                                Debug.Log($"Ignore barcode \"{result.Text}\" while cooldown.");
                            }
                            else
                            {
                                Debug.LogWarning("Barcode read: " + result.Text);

                                if (BarcodeProcessor.Instance != null)
                                {
                                    BarcodeProcessor.Instance.ProcessBarcode(result.Text);
                                    // Set flag to true immediately after sending to processor
                                    _waitingForProcessorResponse = true;
                                    _lastProcessedBarcode = result.Text;
                                    _lastProcessedTime = Time.time;
                                }
                                else
                                {
                                    Debug.LogError("BarcodeProcessor.Instance is NULL when trying to process barcode! Is the BarcodeProcessor GameObject active?");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding the barcode: {ex.GetType().Name} - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }

            yield return new WaitForSeconds(_scanFrequency);
        }
    }

    public void StartScanning()
    {
        if (_webCamTextureManager == null || _webCamTextureManager.WebCamTexture == null || !_webCamTextureManager.WebCamTexture.isPlaying)
        {
            Debug.LogWarning("Cannot start scanning: WebCamTexture is not ready yet.");
            return;
        }

        if (!IsScanning)
        {
            IsScanning = true;
            Debug.Log("Scanning started.");

            if (_scanCoroutine != null)
            {
                StopCoroutine(_scanCoroutine);
            }
            _scanCoroutine = StartCoroutine(ScanLoop());
        }
    }

    public void StopScanning()
    {
        if (IsScanning)
        {
            IsScanning = false;
            Debug.Log("Scanning stopped.");

            if (_scanCoroutine != null)
            {
                StopCoroutine(_scanCoroutine);
                _scanCoroutine = null;
            }
        }
    }

    private void HandleStartScanning(BarcodeScannerType type)
    {
        if (type == BarcodeScannerType.AUTO)
        {
            StartScanning();
        }
    }

    public void HandleStopScanning(BarcodeScannerType type)
    {
        if (type == BarcodeScannerType.AUTO)
        {
            StopScanning();
        }
    }

    private void HandleProductProcessed(bool success, string productNameOrError, Root product)
    {
        _waitingForProcessorResponse = false; // Reset the flag once the processor is done

        if (success)
        {
            Debug.Log($"Product processed successfully: {productNameOrError}. Stopping the scanner.");
            StopScanning();
        }
        else
        {
            Debug.LogWarning($"Product could not be found/processed: {productNameOrError}. Scanning continues...");
        }
    }
}