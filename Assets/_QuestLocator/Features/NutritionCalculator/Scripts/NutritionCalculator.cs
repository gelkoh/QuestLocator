using System;
using System.Collections.Generic;
using UnityEngine;

public class NutritionCalculator : MonoBehaviour
{
    public static NutritionCalculator NutritionCalculatorInstance { get; private set; }
    public Action<NutritionRecommendation> OnNutritionRecommendationCalculated;

    [Header("CSV mit Richtwerten")]
    [SerializeField] private TextAsset _nutritionDataCSV;

    [Header("Default-Werte bei Szenenstart")]
    public int defaultAge = 25;
    public string defaultSex = "Männlich";
    public int defaultWeight = 70;
    public float defaultPAL = 1.5f;

    private int _age;
    private string _sex;
    private int _weight;
    private float _pal;

    private readonly List<NutritionData> nutritionDataList = new();

    void Awake()
    {
        if (NutritionCalculatorInstance == null)
            NutritionCalculatorInstance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadNutritionData();

        _age = defaultAge;
        _sex = defaultSex;
        _weight = defaultWeight;
        _pal = defaultPAL;

        UpdateNutritionRecommendation();
    }

    private void LoadNutritionData()
    {
        if (_nutritionDataCSV == null)
        {
            Debug.LogError("❌ NutritionCalculator: Keine CSV zugewiesen.");
            return;
        }

        string[] lines = _nutritionDataCSV.text.Split('\n');
        Debug.Log($"📥 NutritionCalculator: CSV-Zeilen geladen: {lines.Length}");

        for (int i = 1; i < lines.Length; i++) // Header überspringen
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');
            if (values.Length < 9) continue;

            NutritionData data = new NutritionData
            {
                ageGroup = values[0].Trim(),
                sex = values[1].Trim(),
                pal = ParseFloat(values[2]),
                energyKcal = ParseFloat(values[3]),
                proteinGPerKg = ParseFloat(values[4]), // ✅ z. B. 0.8 → nicht 8
                fatG = ParseFloat(values[5]),
                satFatG = ParseFloat(values[6]),
                carbsG = ParseFloat(values[7]),
                sugarG = ParseFloat(values[8])
            };

            nutritionDataList.Add(data);
        }
    }

    private float ParseFloat(string value)
    {
        value = value.Replace(",", "."); // für Komma statt Punkt
        return float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result) ? result : 0f;
    }

    public void OnSexDropdownValueChanged(int newIndex)
    {
        _sex = newIndex switch
        {
            1 => "Männlich",
            2 => "Weiblich",
            _ => null
        };
        UpdateNutritionRecommendation();
    }

    public void OnAgeSliderValueChanged(float age)
    {
        _age = (int)age;
        UpdateNutritionRecommendation();
    }

    public void OnWeightSliderValueChanged(float weight)
    {
        _weight = (int)weight;
        UpdateNutritionRecommendation();
    }

    public void OnPALSliderValueChanged(float pal)
    {
        _pal = pal / 10f;
        UpdateNutritionRecommendation();
    }

    public void UpdateNutritionRecommendation()
    {
        Debug.Log("🔁 NutritionCalculator: Updating nutrition recommendation.");
        NutritionData matchedData = FindMatchingData();

        if (matchedData != null && _weight > 0 && _pal > 0)
        {
            NutritionRecommendation recommendation = CalculateRecommendation(matchedData);
            Debug.Log($"✅ Tagesbedarf berechnet – kcal: {recommendation.energyKcal}, Protein: {recommendation.proteinG}, Fett: {recommendation.fatG}, Zucker: {recommendation.sugarG}, KH: {recommendation.carbsG}");
            OnNutritionRecommendationCalculated?.Invoke(recommendation);
        }
        else
        {
            Debug.LogWarning("⚠️ NutritionCalculator: Keine gültige Grundlage zur Berechnung.");
        }
    }

    private NutritionData FindMatchingData()
    {
        foreach (var data in nutritionDataList)
        {
            if (IsInAgeGroup(data.ageGroup) && data.sex == _sex && Mathf.Approximately(data.pal, _pal))
            {
                Debug.Log("🎯 Exakte Daten gefunden.");
                return data;
            }
        }

        foreach (var data in nutritionDataList)
        {
            if (IsInAgeGroup(data.ageGroup) && data.sex == _sex)
            {
                Debug.Log("✅ Passende Daten gefunden (PAL abweichend).");
                return data;
            }
        }

        var fallback = nutritionDataList.Find(d =>
            d.ageGroup.Trim().Equals("Default", StringComparison.OrdinalIgnoreCase)
        );

        if (fallback != null)
        {
            Debug.LogWarning("⚠️ Kein exakter Treffer – Default-Werte werden verwendet.");
            return fallback;
        }

        Debug.LogError("❌ Kein Default-Eintrag in CSV gefunden.");
        return null;
    }

    private bool IsInAgeGroup(string ageGroup)
    {
        if (string.IsNullOrWhiteSpace(ageGroup)) return false;

        if (ageGroup.Trim().Equals("Default", StringComparison.OrdinalIgnoreCase)) return false;

        if (ageGroup.EndsWith("+"))
        {
            string ageString = ageGroup.TrimEnd('+');
            if (int.TryParse(ageString, out int minAge))
                return _age >= minAge;
        }
        else
        {
            string[] range = ageGroup.Split('-');
            if (range.Length == 2 &&
                int.TryParse(range[0], out int min) &&
                int.TryParse(range[1], out int max))
            {
                return _age >= min && _age <= max;
            }
        }

        Debug.LogWarning($"⚠️ Altersgruppe '{ageGroup}' konnte nicht geparst werden.");
        return false;
    }

    private NutritionRecommendation CalculateRecommendation(NutritionData data)
    {
        float palAdjustment = _pal / data.pal;

        return new NutritionRecommendation
        {
            energyKcal = data.energyKcal * palAdjustment,
            proteinG = data.proteinGPerKg * _weight, // z. B. 0.8 * 70 = 56g
            fatG = data.fatG * palAdjustment,
            satFatG = data.satFatG * palAdjustment,
            carbsG = data.carbsG * palAdjustment,
            sugarG = data.sugarG * palAdjustment
        };
    }
}
