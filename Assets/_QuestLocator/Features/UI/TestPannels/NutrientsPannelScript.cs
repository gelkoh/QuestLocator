using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class NutrientsPannelScript : MonoBehaviour
{
    private ProductParent productDisplayScript;
    [SerializeField] TextMeshProUGUI productName;
    [SerializeField] GameObject NutrientSection;
    void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        FillInfo();
    }

    private void FillInfo()
    {
        productName.text = productDisplayScript.productData.Product.ProductName;

        createText(NutrientSection, productDisplayScript.productData.Product.Nutriments.EnergyKcal100G.ToString());
        createText(NutrientSection, productDisplayScript.productData.Product.Nutriments.Fat100G.ToString());
        createText(NutrientSection, productDisplayScript.productData.Product.Nutriments.Proteins100G.ToString());
    }
    private void createText(GameObject parent, string name)
    {
        GameObject textObj = new GameObject("IngredientText");
        textObj.transform.SetParent(NutrientSection.transform, false); // 'false' keeps local scale

        // Add TextMeshProUGUI component
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = name;
        tmp.fontSize = 30;
    }

}
