using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Footprint : MonoBehaviour
{
    private ProductParent productDisplayScript;
    [SerializeField] TextMeshProUGUI title;
    
    [SerializeField] TextMeshProUGUI EcoGradeTF;
    [SerializeField] TextMeshProUGUI EcoScoreTF;
    [SerializeField] TextMeshProUGUI Co2TF;
    [SerializeField] GameObject PackagingSection;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //productDisplayScript = GetComponentInParent<ProductParent>();
        //FillInfo();
        
    }

    public void FillInfo()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        title.text = productDisplayScript.productData.Product.ProductName;

        Debug.Log("grade: " + productDisplayScript.productData.Product.EcoscoreData.Grade);
        if (productDisplayScript.productData.Product.EcoscoreData?.Grade != null)
        {
            EcoGradeTF.text = productDisplayScript.productData.Product.EcoscoreData.Grade;
        }
        else
        {
            EcoGradeTF.text = "Data not available";
        }

        Debug.Log("score: " + productDisplayScript.productData.Product.EcoscoreData.Score);
        if (productDisplayScript.productData.Product.EcoscoreData?.Score != null)
        {
            EcoScoreTF.text = productDisplayScript.productData.Product.EcoscoreData.Score.ToString();
        }
        else
        {
            EcoScoreTF.text = "Data not available";
        }

        Debug.Log("co2: " + productDisplayScript.productData.Product.EcoscoreData.Agribalyse.Co2Total);
        if (productDisplayScript.productData.Product.EcoscoreData?.Agribalyse?.Co2Total != null)
        {
            double co2Value = productDisplayScript.productData.Product.EcoscoreData.Agribalyse.Co2Total.Value;
            Co2TF.text = Math.Round(co2Value * 100.0).ToString() + "g of CO2 per 100g of product";
        }
        else
        {
            Co2TF.text = "Data not available";
        }

        if (productDisplayScript.productData.Product.EcoscoreData?.Adjustments?.Packaging?.Packagings != null)
        {
            foreach (var packaging in productDisplayScript.productData.Product.EcoscoreData.Adjustments.Packaging.Packagings)
            {
                GameObject textObj = new GameObject("IngredientText");
                textObj.transform.SetParent(PackagingSection.transform, false); // 'false' keeps local scale

                // Add TextMeshProUGUI component
                var tmp = textObj.AddComponent<TextMeshProUGUI>();
                tmp.text = packaging.Material[3..];

                if (packaging.WeightMeasured != null)
                {
                    tmp.text += ": " + packaging.WeightMeasured + "g";
                }

                tmp.fontSize = 14;
                tmp.alignment = TextAlignmentOptions.MidlineLeft;

            }
        }
        else
        {
            GameObject textObj = new GameObject("IngredientText");
            textObj.transform.SetParent(PackagingSection.transform, false); // 'false' keeps local scale

            // Add TextMeshProUGUI component
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "No packaging information available.";
        }
        

        
    }
}
