using UnityEngine;
using TMPro;
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

    public void InitAfterDataAvailable(NutritionRecommendation recommendation)
    {
        TryFillBars(recommendation);
    }

    private void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        TryFillBars(NutritionCalculatorInstance.CurrentNutritionRecommendation);
    }

    private void TryFillBars(NutritionRecommendation nutritionRecommendation)
    {
        Debug.Log("🔁 [NutrientsPannelScript] FillBars() wird aufgerufen...");

        if (productDisplayScript == null || productDisplayScript.productData?.Product == null)
        {
            Debug.LogError("❌ [NutrientsPannelScript] Produktdaten nicht verfügbar.");
            return;
        }

        if (nutrientBarFiller == null)
        {
            Debug.LogError("❌ [NutrientsPannelScript] nutrientBarFiller ist nicht zugewiesen! Bitte im Inspector setzen.");
            return;
        }

        if (productName != null)
        {
            productName.text = productDisplayScript.productData.Product.ProductName;
        }

        // Fülle die Balken mit dem aktiven Modus
        nutrientBarFiller.FillBars(productDisplayScript.productData.Product, nutritionRecommendation);
    }

    private void HandleNutritionRecommendationCalculated(NutritionRecommendation nutritionRecommendation)
    {
        TryFillBars(nutritionRecommendation);
    }
}
