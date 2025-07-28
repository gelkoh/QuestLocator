using UnityEngine;
using TMPro;
using static NutritionCalculator;
using static DisplayModeManager;

public class NutrientsPannelScript : MonoBehaviour
{
    [SerializeField] private NutrientBarFiller nutrientBarFiller;
    [SerializeField] private TextMeshProUGUI productName;
    [SerializeField] private TextMeshProUGUI activeModeText;

    private ProductParent productDisplayScript;

    void OnEnable()
    {
        NutritionCalculatorInstance.OnNutritionRecommendationCalculated += HandleNutritionRecommendationCalculated;
        DisplayModeManagerInstance.OnDisplayModeChanged += HandleDisplayModeChanged;
    }

    void OnDisable()
    {
        NutritionCalculatorInstance.OnNutritionRecommendationCalculated -= HandleNutritionRecommendationCalculated;
        DisplayModeManagerInstance.OnDisplayModeChanged -= HandleDisplayModeChanged;
    }

    public void InitAfterDataAvailable(NutritionRecommendation recommendation)
    {
        TryFillBars(recommendation);
    }

    private void Start()
    {
        Debug.LogError("start nutrients");
        productDisplayScript = GetComponentInParent<ProductParent>();
        TryFillBars(NutritionCalculatorInstance.CurrentNutritionRecommendation);

        activeModeText.SetText(DisplayModeManagerInstance.GetTextForMode(DisplayModeManagerInstance.CurrentMode));
    }

    private void TryFillBars(NutritionRecommendation nutritionRecommendation)
    {

        if (productDisplayScript == null || productDisplayScript.productData?.Product == null)
        {
            Debug.LogError(" [NutrientsPannelScript] Produktdaten nicht verfügbaree.");
            return;
        }

        if (nutrientBarFiller == null)
        {
            Debug.LogError(" [NutrientsPannelScript] nutrientBarFiller ist nicht zugewiesen! Bitte im Inspector setzen.");
            return;
        }

        if (productName != null)
        {
            productName.text = productDisplayScript.productData.Product.ProductName;
        }

        nutrientBarFiller.FillBars(productDisplayScript.productData.Product, nutritionRecommendation);
    }

    private void HandleNutritionRecommendationCalculated(NutritionRecommendation nutritionRecommendation)
    {
        TryFillBars(nutritionRecommendation);
    }

    private void HandleDisplayModeChanged(DisplayMode newMode)
    {
        Debug.Log("[NutrientsPannelScript]: Setting new mode to: " + newMode.ToString());

        var newModeText = newMode switch
        {
            DisplayMode.Per100gVsNeed => "100g vs Daily Need",
            DisplayMode.PerPortionVsNeed => "Portion vs Daily Need",
            DisplayMode.Per100gVsLimit => "100g vs Recommended Limit",
            _ => "Unknown Mode"
        };

        Debug.Log("[NutrientsPannelScript]: newModeText " + newModeText);

        activeModeText.SetText(newModeText);
        TryFillBars(NutritionCalculatorInstance.CurrentNutritionRecommendation);
    }
}