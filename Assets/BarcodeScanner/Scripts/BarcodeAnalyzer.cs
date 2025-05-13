using UnityEngine;
using ZXing;
using System;

public class BarcodeAnalyzer : MonoBehaviour
{
    //public OpenFoo openFoodFactsApi;
    private BarcodeReader m_barcodeReader;

    public BarcodeAnalyzer()
    {
        try
        {
            m_barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    //TryInverted = true,
                    // PossibleFormats = new List<BarcodeFormat> {
                    //     BarcodeFormat.EAN_8,
                    //     BarcodeFormat.EAN_13
                    // }
                }
            };
        }
        catch (Exception exception)
        {
            Debug.LogError("ZXing barcode reader could not be initialised: " + exception.Message);
            m_barcodeReader = null;
        }
    }

    public string Analyze(Frame barcodeFrame)
    {
        try 
        {
            var result = m_barcodeReader.Decode(barcodeFrame.Pixels, barcodeFrame.Width, barcodeFrame.Height);
            return result?.Text;
        }
        catch (Exception exception)
        {
            Debug.LogError("❌ Error while scanning: " + exception);
            return null;
        }
    }
    // public bool Analyze(Frame barcodeFrame)
    // {
    //     // If EAN could not be found, analysing should start again...
    //     try
    //     {
    //         var ean = m_barcodeReader.Decode(barcodeFrame.Pixels, barcodeFrame.Width, barcodeFrame.Height);

    //         if (ean != null)
    //         {
    //             Debug.LogWarning("✅ Barcode read successfully: " + ean.Text);
                
    //             StartCoroutine(_openFoodFactsClient.GetProductByEan(ean.Text,
    //                 onSuccess: (root) =>
    //                 {
    //                     Debug.Log($"Produkt-ID: {root.product._id}");
    //                     Debug.Log($"Keywords: {string.Join(", ", root.product._keywords)}");
    //                 },
    //                 onError: (error) =>
    //                 {
    //                     Debug.LogError($"Fehler beim Abruf: {error}");
    //                 }));

    //             return true;
    //         }
    //         else
    //         {
    //             Debug.LogWarning("🚫 Barcode could not be read");
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError("❌ Error while scanning: " + ex);
    //     }

    //     return false;
    // }
}