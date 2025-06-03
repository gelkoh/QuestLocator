using UnityEngine;
using static BarcodeScannerStatusManager;
using static BarcodeScannerEventManager;
using static BarcodeProcessor;

public class BarcodeScannerUIController : MonoBehaviour
{
    [SerializeField] private GameObject _barcodeScannerStatusCanvas;
    [SerializeField] private GameObject _barcodeManualScannerScanFrame;

    void OnEnable()
    {
        ////// DOES THIS GO HERE?!?!?!
        _barcodeScannerStatusCanvas.SetActive(false);
        _barcodeManualScannerScanFrame.SetActive(false);

        if (BarcodeScannerStatusManagerInstance != null)
        {
            BarcodeScannerStatusManagerInstance.OnScannerStatusChanged += HandleScannerStatusChanged;
            // BarcodeProcessorInstance.OnProductProcessed += HandleBarcodeProcessed;
        }
        else
        {
            Debug.LogWarning("BarcodeScannerUIController: BarcodeScannerStatusManager Instance not found.");
        }
    }

    void OnDisable()
    {
        if (BarcodeScannerStatusManagerInstance != null)
        {
            BarcodeScannerStatusManagerInstance.OnScannerStatusChanged -= HandleScannerStatusChanged;
            // BarcodeProcessorInstance.OnProductProcessed -= HandleBarcodeProcessed;
        }
    }

    private void HandleScannerStatusChanged(bool isActive, BarcodeScannerType type)
    {
        if (isActive == true)
        {
            _barcodeScannerStatusCanvas.SetActive(true);

            if (type == BarcodeScannerType.MANUAL)
            {
                _barcodeManualScannerScanFrame.SetActive(true);
            }
        }
        else if (isActive == false)
        {
            _barcodeScannerStatusCanvas.SetActive(false);

            if (_barcodeManualScannerScanFrame.activeSelf)
            {
                _barcodeManualScannerScanFrame.SetActive(false);
            }
        }
    }

    // private void HandleBarcodeProcessed(bool b, string s, Root productData)
    // {
    //     // ALSO CLOSE UI HERE
    //     Debug.Log($"UI: Scanned product: {productData.Product.ProductName}");
    // }
}
