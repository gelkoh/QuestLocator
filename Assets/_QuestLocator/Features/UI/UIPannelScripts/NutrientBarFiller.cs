using System.Collections.Generic;
using UnityEngine;

public class NutrientBarFiller : MonoBehaviour
{
    [SerializeField] private List<NutrientBarUI> nutrientBars;

    public void FillBars(Nutriments n, NutritionRecommendation r)
    {
        Debug.Log($"🔵 FillBars gestartet – Energie: {n.EnergyKcal100G}, Empfohlen (Tagesbedarf): {r.energyKcal}");

        float kcalPct = (float)n.EnergyKcal100G / r.energyKcal;
        float sugarPct = (float)n.Sugars100G / r.sugarG;
        float fatPct = (float)n.Fat100G / r.fatG;
        float satFatPct = (float)n.SaturatedFat100G / r.satFatG;
        float proteinPct = (float)n.Proteins100G / r.proteinG;
        float carbsPct = (float)n.Carbohydrates100G / r.carbsG;

        nutrientBars[0].SetData("Energie", (float)n.EnergyKcal100G, r.energyKcal, "kcal");
        Debug.Log($"🟦 Energie: {n.EnergyKcal100G}, Tagesbedarf: {r.energyKcal}, ≈ {kcalPct * 100f:0.0}%");

        nutrientBars[1].SetData("Zucker", (float)n.Sugars100G, r.sugarG, "g");
        Debug.Log($"🟥 Zucker: {n.Sugars100G}, Tagesbedarf: {r.sugarG}, ≈ {sugarPct * 100f:0.0}%");

        nutrientBars[2].SetData("Fett", (float)n.Fat100G, r.fatG, "g");
        Debug.Log($"🟥 Fett: {n.Fat100G}, Tagesbedarf: {r.fatG}, ≈ {fatPct * 100f:0.0}%");

        nutrientBars[3].SetData("Ges. Fett", (float)n.SaturatedFat100G, r.satFatG, "g");
        Debug.Log($"🟥 Ges. Fett: {n.SaturatedFat100G}, Tagesbedarf: {r.satFatG}, ≈ {satFatPct * 100f:0.0}%");

        nutrientBars[4].SetData("Proteine", (float)n.Proteins100G, r.proteinG, "g");
        Debug.Log($"🟥 Proteine: {n.Proteins100G}, Tagesbedarf: {r.proteinG}, ≈ {proteinPct * 100f:0.0}%");

        nutrientBars[5].SetData("Kohlenhydrate", (float)n.Carbohydrates100G, r.carbsG, "g");
        Debug.Log($"🟥 Kohlenhydrate: {n.Carbohydrates100G}, Tagesbedarf: {r.carbsG}, ≈ {carbsPct * 100f:0.0}%");
    }
}
