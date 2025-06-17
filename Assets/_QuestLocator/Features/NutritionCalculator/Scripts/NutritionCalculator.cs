using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NutritionCalculator : MonoBehaviour
{
    // Singleton
    public static NutritionCalculator NutritionCalculatorInstance { get; private set; }
    public Action<NutritionRecommendation> OnNutritionRecommendationCalculated;

    [SerializeField] private TextAsset _nutritionDataCSV;

    private int _age = -1;
    private string _sex = null;
    private int _weight = -1;
    private float _pal = -1.0f;

    // Stores the parsed nutrition data
    private readonly List<NutritionData> nutritionDataList = new();

    void Awake()
    {
        if (NutritionCalculatorInstance == null)
        {
            NutritionCalculatorInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadNutritionData();
    }

    private NutritionData FindMatchingData()
    {
        Debug.Log("NutritionCalculator: Finding matching data.");

        foreach (var data in nutritionDataList)
        {
            Debug.Log($"NutritionCalculator: data.ageGroup: {data.ageGroup}, data.set: {data.sex} ");
            // Check if this data matches our age group and sex
            if (IsInAgeGroup(data.ageGroup) && data.sex == _sex)
            {
                // Found a match for age group and sex
                Debug.Log("NutritionCalculator: Found matching data.");
                return data;
            }
        }

        Debug.LogWarning("NutritionCalculator: Couldn't find matching data.");
        return null;
    }

    private bool IsInAgeGroup(string ageGroup)
    {
        if (string.IsNullOrEmpty(ageGroup)) return false;

        if (ageGroup.EndsWith("+"))
        {
            string ageString = ageGroup.TrimEnd('+');
            if (int.TryParse(ageString, out int minAge))
            {
                return _age >= minAge;
            }
        }
        else
        {
            string[] range = ageGroup.Split('-');
            if (range.Length == 2)
            {
                if (int.TryParse(range[0], out int min) && int.TryParse(range[1], out int max))
                {
                    return _age >= min && _age <= max;
                }
            }
        }
        
        Debug.LogWarning($"NutritionCalculator: Could not parse age group '{ageGroup}'. Returning false.");
        return false;
    }

    private NutritionRecommendation CalculateRecommendation(NutritionData data)
    {
        float palAdjustment = _pal / data.pal;

        NutritionRecommendation recommendation = new()
        {
            energyKcal = data.energyKcal * palAdjustment,
            proteinG = data.proteinGPerKg * _weight,
            fatG = data.fatG * palAdjustment,
            satFatG = data.satFatG * palAdjustment,
            carbsG = data.carbsG * palAdjustment,
            sugarG = data.sugarG * palAdjustment
        };

        return recommendation;
    }

    public void OnSexDropdownValueChanged(int newIndex)
    {        
        if (newIndex == 1)
        {
            _sex = "Männlich";
        }
        else if (newIndex == 2)
        {
            _sex = "Weiblich";
        }
        else
        {
            _sex = null;
        }

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
        _pal = pal / 10;
        UpdateNutritionRecommendation();
    }

    private void UpdateNutritionRecommendation()
    {
        Debug.Log("NutritionCalculator: Updating nutrition recommendation.");
        NutritionData matchedData = FindMatchingData();

        if (matchedData != null && _weight != -1 && _pal != -1.0f)
        {
            NutritionRecommendation nutritionRecommendation = CalculateRecommendation(matchedData);
            OnNutritionRecommendationCalculated?.Invoke(nutritionRecommendation);
        }
    }

    private void LoadNutritionData()
    {
        if (_nutritionDataCSV == null)
        {
            Debug.LogError("NutritionCalculator: Nutrition CSV file is not assigned in the inspector!");
            return;
        }

        string[] lines = _nutritionDataCSV.text.Split('\n');
        Debug.Log($"lines: {lines}");

        // Skip the header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');
            
            if (values.Length < 9) continue;

            NutritionData data = new()
            {
                ageGroup = values[0].Trim(),
                sex = values[1].Trim(),
                pal = ParseFloat(values[2]),
                energyKcal = ParseFloat(values[3]),
                proteinGPerKg = ParseFloat(values[4]),
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
        if (float.TryParse(value, out float result))
        {
            return result;
        }

        return 0f;
    }
}