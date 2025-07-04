using UnityEngine;
using TMPro;
using System.Collections;
using static NutritionCalculator;

public class NutrientsPannelScript : MonoBehaviour
{
    [SerializeField] private NutrientBarFiller nutrientBarFiller;
    [SerializeField] private TextMeshProUGUI productName;

    private ProductParent productDisplayScript;

    void OnEnable()
    {
        NutritionCalculatorInstance.OnNutritionRecommendationCalculated += HandleNutritionRecommendationCalculated;
    }

    void OnDisable()
    {
        NutritionCalculatorInstance.OnNutritionRecommendationCalculated -= HandleNutritionRecommendationCalculated;
    }

    private void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        TryFillBars(NutritionCalculatorInstance.CurrentNutritionRecommendation);
    }

    private void TryFillBars(NutritionRecommendation nutritionRecommendation)
    {
        Debug.Log("FillBars() wird aufgerufen...");
        productName.text = productDisplayScript.productData.Product.ProductName;

        if (nutrientBarFiller == null)
        {
            Debug.LogError(" nutrientBarFiller ist nicht zugewiesen! Bitte das GameObject mit dem Script (z. B. col1) im Inspector eintragen.");
            return;
        }

        /* if (productData?.Product?.Nutriments == null)
        {
            Debug.LogWarning("Keine Nährwertdaten verfügbar.");
            return;
        } */
        if (productDisplayScript.productData.Product.Nutriments == null)
        {
            Debug.LogError("Keine Nährwertdaten verfügbar.");
            return;
        }

        nutrientBarFiller.FillBars(productDisplayScript.productData.Product.Nutriments, nutritionRecommendation);
    }

    private void HandleNutritionRecommendationCalculated(NutritionRecommendation nutritionRecommendation)
    {
        TryFillBars(nutritionRecommendation);
    }
}