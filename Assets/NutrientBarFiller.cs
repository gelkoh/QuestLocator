using System.Collections.Generic;
using UnityEngine;

public class NutrientBarFiller : MonoBehaviour
{
    [SerializeField] private List<NutrientBarUI> nutrientBars;

    public void FillBars(Nutriments n, NutritionRecommendation r)
    {
        Debug.Log($"🔵 FillBars gestartet – Energie: {n.EnergyKcal100G}, Empfohlen (Tagesbedarf): {r.energyKcal}");

        nutrientBars[0].SetData("Energie", (float)n.EnergyKcal100G, r.energyKcal, "kcal");
        nutrientBars[1].SetData("Zucker", (float)n.Sugars100G, r.sugarG, "g");
        nutrientBars[2].SetData("Fett", (float)n.Fat100G, r.fatG, "g");
        nutrientBars[3].SetData("Ges. Fett", (float)n.SaturatedFat100G, r.satFatG, "g");
        nutrientBars[4].SetData("Proteine", (float)n.Proteins100G, r.proteinG, "g");
        nutrientBars[5].SetData("Kohlenhydrate", (float)n.Carbohydrates100G, r.carbsG, "g");

        foreach (var bar in nutrientBars)
        {
            bar.InitializeAndAnimate();
        }

        Debug.Log($"🟦 Energie: {n.EnergyKcal100G}, Tagesbedarf: {r.energyKcal}, ≈ {((float)n.EnergyKcal100G / r.energyKcal) * 100f:0.0}%");
        Debug.Log($"🟥 Zucker: {n.Sugars100G}, Tagesbedarf: {r.sugarG}, ≈ {((float)n.Sugars100G / r.sugarG) * 100f:0.0}%");
        Debug.Log($"🟥 Fett: {n.Fat100G}, Tagesbedarf: {r.fatG}, ≈ {((float)n.Fat100G / r.fatG) * 100f:0.0}%");
        Debug.Log($"🟥 Ges. Fett: {n.SaturatedFat100G}, Tagesbedarf: {r.satFatG}, ≈ {((float)n.SaturatedFat100G / r.satFatG) * 100f:0.0}%");
        Debug.Log($"🟥 Proteine: {n.Proteins100G}, Tagesbedarf: {r.proteinG}, ≈ {((float)n.Proteins100G / r.proteinG) * 100f:0.0}%");
        Debug.Log($"🟥 Kohlenhydrate: {n.Carbohydrates100G}, Tagesbedarf: {r.carbsG}, ≈ {((float)n.Carbohydrates100G / r.carbsG) * 100f:0.0}%");
    }
}