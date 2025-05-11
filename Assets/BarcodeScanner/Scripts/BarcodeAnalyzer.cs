using UnityEngine;
using ZXing;
using System.Collections.Generic;
using System;

public class BarcodeAnalyzer
{
    public OpenFoodFactsAPI openFoodFactsApi;
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

    public void Analyze(Frame barcodeFrame)
    {
        // If EAN could not be found, analysing should start again...
        try
        {
            var result = m_barcodeReader.Decode(barcodeFrame.Pixels, barcodeFrame.Width, barcodeFrame.Height);

            if (result != null)
            {
                Debug.LogWarning("✅ Barcode read successfully: " + result.Text);
                //openFoodFactsApi.MakeAPICall(result.Text);
            }
            else
            {
                Debug.LogWarning("🚫 Barcode could not be read");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error while scanning: " + ex);
        }
    }
}