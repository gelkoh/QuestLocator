using System.Collections.Generic;
using UnityEngine;

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

        Debug.Log($"Berechne Balken für Modus: {DisplayModeManager.Instance.CurrentMode}");

        switch (DisplayModeManager.Instance.CurrentMode)
        {
            case DisplayMode.Per100gVsNeed:
                Fill("Energie", (float)n.EnergyKcal100G, recommendation.energyKcal, "kcal", 0);
                Fill("Zucker", (float)n.Sugars100G, recommendation.sugarG, "g", 1);
                Fill("Fett", (float)n.Fat100G, recommendation.fatG, "g", 2);
                Fill("Ges. Fett", (float)n.SaturatedFat100G, recommendation.satFatG, "g", 3);
                Fill("Proteine", (float)n.Proteins100G, recommendation.proteinG, "g", 4);
                Fill("Kohlenhydrate", (float)n.Carbohydrates100G, recommendation.carbsG, "g", 5);
                break;

            case DisplayMode.PerPortionVsNeed:
                Fill("Energie", (float)(n.EnergyKcal100G * portionFactor), recommendation.energyKcal, "kcal", 0);
                Fill("Zucker", (float)(n.Sugars100G * portionFactor), recommendation.sugarG, "g", 1);
                Fill("Fett", (float)(n.Fat100G * portionFactor), recommendation.fatG, "g", 2);
                Fill("Ges. Fett", (float)(n.SaturatedFat100G * portionFactor), recommendation.satFatG, "g", 3);
                Fill("Proteine", (float)(n.Proteins100G * portionFactor), recommendation.proteinG, "g", 4);
                Fill("Kohlenhydrate", (float)(n.Carbohydrates100G * portionFactor), recommendation.carbsG, "g", 5);
                break;

            case DisplayMode.Per100gVsLimit:
                Fill("Energie", (float)n.EnergyKcal100G, StaticReferenceValues.MaxEnergyPer100g, "kcal", 0);
                Fill("Zucker", (float)n.Sugars100G, StaticReferenceValues.MaxSugarPer100g, "g", 1);
                Fill("Fett", (float)n.Fat100G, StaticReferenceValues.MaxFatPer100g, "g", 2);
                Fill("Ges. Fett", (float)n.SaturatedFat100G, StaticReferenceValues.MaxSatFatPer100g, "g", 3);
                Fill("Proteine", (float)n.Proteins100G, StaticReferenceValues.MaxProteinPer100g, "g", 4);
                Fill("Kohlenhydrate", (float)n.Carbohydrates100G, StaticReferenceValues.MaxCarbsPer100g, "g", 5);
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
