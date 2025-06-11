using System;
using System.Collections;
using UnityEngine;
using static BarcodeScannerStatusManager;
using static SoundFeedbackManager;
using static ScanHistoryManager;

public class BarcodeProcessor : MonoBehaviour
{
    // Singleton
    public static BarcodeProcessor BarcodeProcessorInstance { get; private set; }

    public event Action<bool, string, Root> OnProductProcessed;

    private OpenFoodFactsClient _openFoodFactsClient;
    private bool _isProcessing = false;

    private void Awake()
    {
        if (BarcodeProcessorInstance == null)
        {
            BarcodeProcessorInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        while (BarcodeProcessorInstance == null)
        {
            Debug.Log("Waiting for BarcodeProcessor to initialize...");
            yield return null;
        }

        _openFoodFactsClient = new OpenFoodFactsClient();
    }

    public void ProcessBarcode(string barcode)
    {
        if (_isProcessing)
        {
            Debug.LogWarning("Barcode-Verarbeitung läuft bereits. Barcode wird ignoriert: " + barcode);
            return;
        }

        _isProcessing = true;
        StartCoroutine(GetProductData(barcode));
    }

    public IEnumerator GetProductData(string barcode)
    {
        Debug.LogError("InGetProductData---");
        yield return new WaitForSeconds(0.25f);

        Debug.Log($"Anfrage an OpenFoodFacts für EAN: {barcode}");

        StartCoroutine(_openFoodFactsClient.GetProductByEan(barcode,
            onSuccess: (productRoot) =>
            {
                if (productRoot != null && productRoot.Product != null && productRoot.Status == 1)
                {
                    Debug.LogWarning($"Produkt gefunden: {productRoot.Product.ProductName}");
                    OnProductProcessed?.Invoke(true, productRoot.Product.ProductName, productRoot);
                    BarcodeScannerEventManager.StopScanning(BarcodeScannerStatusManagerInstance.ActiveScannerType);
                    SoundFeedbackManagerInstance.PlayScanSuccess();

                    ScanHistoryManagerInstance.AddProductAndSave(productRoot);
                }
                else
                {
                    string errorMessage = productRoot != null ? productRoot.StatusVerbose : "Unbekannter API-Fehler";
                    Debug.LogError($"Produkt nicht gefunden für EAN {barcode}: {errorMessage}");
                    OnProductProcessed?.Invoke(false, errorMessage, null);
                    SoundFeedbackManagerInstance.PlayScanFailed();
                }
                _isProcessing = false;
            },
            onError: (err) =>
            {
                Debug.LogError($"Fehler bei OpenFoodFacts Anfrage für EAN {barcode}: {err}");
                OnProductProcessed?.Invoke(false, $"API Error: {err}", null);
                _isProcessing = false;
                SoundFeedbackManagerInstance.PlayScanFailed();
            }
        ));
    }
}