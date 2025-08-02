using System.Collections.Generic;
using UnityEngine;
using static DisplayModeManager;
using static NutritionCalculator;

public class NutrientBarFiller : MonoBehaviour
{
    [SerializeField] private List<NutrientBarUI> nutrientBars;

    public void FillBars(Product product, NutritionRecommendation recommendation)
    {
        if (product == null || product.Nutriments == null)
        {
            Debug.LogError("[NutrientBarFiller] Produkt oder Nährwerte fehlen!");
            return;
        }

        Nutriments n = product.Nutriments;

        float portionFactor = GetPortionFactor(product);

        Debug.Log($"Berechne Balken für Modus: {DisplayModeManagerInstance.CurrentMode}");

        switch (DisplayModeManagerInstance.CurrentMode)
        {
            case DisplayMode.Per100gVsNeed:
                Fill("Energy", (float)n.EnergyKcal100G, recommendation.energyKcal, "kcal", 0);
                Fill("Sugar", (float)n.Sugars100G, recommendation.sugarG, "g", 1);
                Fill("Fat", (float)n.Fat100G, recommendation.fatG, "g", 2);
                Fill("Sat. Fat", (float)n.SaturatedFat100G, recommendation.satFatG, "g", 3);
                Fill("Proteins", (float)n.Proteins100G, recommendation.proteinG, "g", 4);
                Fill("Carbs", (float)n.Carbohydrates100G, recommendation.carbsG, "g", 5);
                break;

            case DisplayMode.PerPortionVsNeed:
                Fill("Energy", (float)(n.EnergyKcal100G * portionFactor), recommendation.energyKcal, "kcal", 0);
                Fill("Sugar", (float)(n.Sugars100G * portionFactor), recommendation.sugarG, "g", 1);
                Fill("Fat", (float)(n.Fat100G * portionFactor), recommendation.fatG, "g", 2);
                Fill("Sat. Fat", (float)(n.SaturatedFat100G * portionFactor), recommendation.satFatG, "g", 3);
                Fill("Proteins", (float)(n.Proteins100G * portionFactor), recommendation.proteinG, "g", 4);
                Fill("Carbs", (float)(n.Carbohydrates100G * portionFactor), recommendation.carbsG, "g", 5);
                break;

            case DisplayMode.Per100gVsLimit:
                var maxEnergyPer100g = 500f;

                var mealsPerDay = 3f;

                var maxSugarPercentageOfKcal = 0.1f;
                var caloriesPerGSugar = 4f;
                var maxSugarPer100g = recommendation.energyKcal * maxSugarPercentageOfKcal / caloriesPerGSugar / mealsPerDay;

                var maxFatPercentageOfKcal = 0.325f;
                var caloriesPerGFat = 9f;
                var maxFatPer100g = recommendation.energyKcal * maxFatPercentageOfKcal / caloriesPerGFat / mealsPerDay;

                var maxSaturatedFatPercentageOfKcal = 0.1f;
                var caloriesPerGSaturatedFat = 9f;
                var maxSaturatedFatPer100G = recommendation.energyKcal * maxSaturatedFatPercentageOfKcal / caloriesPerGSaturatedFat / mealsPerDay;

                var maxProteinPer100g = NutritionCalculatorInstance.CurrentWeight * 0.8f;

                var maxCarbPercentageOfKcal = 0.55f;
                var caloriesPerGCarb = 4f;
                var maxCarbsPer100G = recommendation.energyKcal * maxCarbPercentageOfKcal / caloriesPerGCarb / mealsPerDay;
            
                Fill("Energy", (float)n.EnergyKcal100G, maxEnergyPer100g, "kcal", 0);
                Fill("Sugar", (float)n.Sugars100G, maxSugarPer100g, "g", 1);
                Fill("Fat", (float)n.Fat100G, maxFatPer100g, "g", 2);
                Fill("Sat. Fat", (float)n.SaturatedFat100G, maxSaturatedFatPer100G, "g", 3);
                Fill("Proteins", (float)n.Proteins100G, maxProteinPer100g, "g", 4);
                Fill("Carbs", (float)n.Carbohydrates100G, maxCarbsPer100G, "g", 5);
                break;
        }

        foreach (var bar in nutrientBars)
        {
            bar.InitializeAndAnimate();
        }
    }

    private void Fill(string name, float actual, float max, string unit, int index)
    {
        if (index < 0 || index >= nutrientBars.Count)
        {
            Debug.LogError($"[NutrientBarFiller] Ungültiger Index {index} für Balken '{name}'");
            return;
        }

        nutrientBars[index].SetData(name, actual, max, unit);
    }

    private float GetPortionFactor(Product p)
    {
        if (p.ServingSizeG > 0)
            return p.ServingSizeG / 100f;

        Debug.LogWarning("[NutrientBarFiller] Portion nicht angegeben. Verwende 30g als Default.");
        return StaticReferenceValues.DefaultPortionSize / 100f;
    }
}
