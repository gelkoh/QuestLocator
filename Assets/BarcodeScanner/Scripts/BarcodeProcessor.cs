using System;
using System.Collections;
using UnityEngine;

public class BarcodeProcessor : MonoBehaviour
{
    public static BarcodeProcessor Instance { get; private set; }

    public event Action<bool, string, Root> OnProductProcessed;

    private OpenFoodFactsClient _openFoodFactsClient;
    private bool _isProcessing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want the processor to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        while (Instance == null)
        {
            Debug.Log("Waiting for BarcodeProcessor to initialize...");
            yield return null;
        }

        _openFoodFactsClient = new OpenFoodFactsClient();
    }

    public void ProcessBarcode(string barcode)
    {
        Debug.LogError("INSIDE PROCESS BARCODE");

        if (_isProcessing)
        {
            Debug.LogWarning("Barcode-Verarbeitung läuft bereits. Barcode wird ignoriert: " + barcode);
            return;
        }

        if (barcode.Length != 13) 
        {
            Debug.LogError($"Ungültige EAN-Länge '{barcode}' ({barcode.Length} Ziffern). Erwartet: 13.");
            OnProductProcessed?.Invoke(false, "Invalid EAN length", null);
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(barcode, @"^\d+$"))
        {
            Debug.LogError($"Barcode '{barcode}' enthält nicht-numerische Zeichen. Erwartet nur Ziffern.");
            OnProductProcessed?.Invoke(false, "Non-numeric barcode", null);
            return;
        }

        _isProcessing = true;
        StartCoroutine(GetProductData(barcode));
    }

    public IEnumerator GetProductData(string barcode)
    {
        Debug.LogError("InGetProductData");
        yield return new WaitForSeconds(0.25f);

        Debug.Log($"Anfrage an OpenFoodFacts für EAN: {barcode}");

        StartCoroutine(_openFoodFactsClient.GetProductByEan(barcode,
            onSuccess: (root) =>
            {
                if (root != null && root.Product != null && root.Status == 1)
                {
                    Debug.LogWarning($"Produkt gefunden: {root.Product.ProductName}");
                    OnProductProcessed?.Invoke(true, root.Product.ProductName, root);
                    BarcodeScanEventManager.StopScanning(BarcodeScanEventManager.BarcodeScannerType.AUTO);
                }
                else
                {
                    string errorMessage = root != null ? root.StatusVerbose : "Unbekannter API-Fehler";
                    Debug.LogError($"Produkt nicht gefunden für EAN {barcode}: {errorMessage}");
                    OnProductProcessed?.Invoke(false, errorMessage, null);
                }
                _isProcessing = false;
            },
            onError: (err) =>
            {
                Debug.LogError($"Fehler bei OpenFoodFacts Anfrage für EAN {barcode}: {err}");
                OnProductProcessed?.Invoke(false, $"API Error: {err}", null);
                _isProcessing = false;
            }
        ));
    }
}