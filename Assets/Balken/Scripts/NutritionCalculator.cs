using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NutritionCalculator : MonoBehaviour
{
    [Header("CSV Settings")]
    [SerializeField] private TextAsset nutritionDataCSV;

    [Header("User Input")]
    [SerializeField] private int age = 25;
    [SerializeField] private string sex = "Männlich"; // "Männlich" or "Weiblich"
    [SerializeField] private float weight = 70f; // in kg
    [SerializeField] private float physicalActivityLevel = 1.4f; // PAL value

    // Stores the parsed nutrition data
    private List<NutritionData> nutritionDataList = new List<NutritionData>();
    private NutritionData defaultData;

    [System.Serializable]
    public class NutritionData
    {
        public string ageGroup;
        public string sex;
        public float pal;
        public float energyKcal;
        public float proteinGPerKg;
        public float fatG;
        public float satFatG;
        public float carbsG;
        public float sugarG;
    }

    private void Awake()
    {
        LoadNutritionData();
    }

    private void LoadNutritionData()
    {
        if (nutritionDataCSV == null)
        {
            Debug.LogError("Nutrition CSV file not assigned!");
            return;
        }

        string[] lines = nutritionDataCSV.text.Split('\n');

        // Skip the header
        for (int i = 1; i < lines.Length; i++)
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
                proteinGPerKg = ParseFloat(values[4]),
                fatG = ParseFloat(values[5]),
                satFatG = ParseFloat(values[6]),
                carbsG = ParseFloat(values[7]),
                sugarG = ParseFloat(values[8])
            };

            if (data.ageGroup == "Default")
            {
                defaultData = data;
            }
            else
            {
                nutritionDataList.Add(data);
            }
        }

        Debug.Log($"Loaded {nutritionDataList.Count} nutrition data entries (plus default)");
    }

    private float ParseFloat(string value)
    {
        if (float.TryParse(value, out float result))
        {
            return result;
        }
        return 0f;
    }

    // Call this method to get nutrition recommendations
    public NutritionRecommendation GetNutritionRecommendation()
    {
        // Find the appropriate data based on age and sex
        NutritionData matchedData = FindMatchingData();

        // Calculate recommendations
        return CalculateRecommendation(matchedData);
    }

    private NutritionData FindMatchingData()
    {
        foreach (var data in nutritionDataList)
        {
            // Check if this data matches our age group and sex
            if (IsInAgeGroup(data.ageGroup) && data.sex == sex)
            {
                // Found a match for age group and sex
                return data;
            }
        }

        // If no match found, return default data
        Debug.LogWarning("No matching nutrition data found for the current age and sex. Using default values.");
        return defaultData;
    }

    private bool IsInAgeGroup(string ageGroup)
    {
        if (string.IsNullOrEmpty(ageGroup)) return false;

        string[] range = ageGroup.Split('-');
        if (range.Length != 2) return false;

        if (int.TryParse(range[0], out int min) && int.TryParse(range[1], out int max))
        {
            return age >= min && age <= max;
        }

        return false;
    }

    private NutritionRecommendation CalculateRecommendation(NutritionData data)
    {
        // Adjust energy based on actual PAL if needed
        float palAdjustment = physicalActivityLevel / data.pal;

        NutritionRecommendation recommendation = new NutritionRecommendation
        {
            energyKcal = data.energyKcal * palAdjustment,
            proteinG = data.proteinGPerKg * weight,
            fatG = data.fatG * palAdjustment,
            satFatG = data.satFatG * palAdjustment,
            carbsG = data.carbsG * palAdjustment,
            sugarG = data.sugarG * palAdjustment
        };

        return recommendation;
    }

    // Class to hold the final calculated recommendations
    [System.Serializable]
    public class NutritionRecommendation
    {
        public float energyKcal;
        public float proteinG;
        public float fatG;
        public float satFatG;
        public float carbsG;
        public float sugarG;

        public override string ToString()
        {
            return $"Daily Recommendations:\n" +
                   $"Energy: {energyKcal:F1} kcal\n" +
                   $"Protein: {proteinG:F1} g\n" +
                   $"Fat: {fatG:F1} g\n" +
                   $"Saturated Fat: {satFatG:F1} g\n" +
                   $"Carbohydrates: {carbsG:F1} g\n" +
                   $"Sugar: {sugarG:F1} g";
        }
    }

    // Example of how to use this in your game
    public void PrintNutritionRecommendation()
    {
        NutritionRecommendation recommendation = GetNutritionRecommendation();
        Debug.Log(recommendation.ToString());
    }

    // Optional: Example UI methods
    public void SetAge(string ageStr)
    {
        if (int.TryParse(ageStr, out int newAge))
        {
            age = newAge;
        }
    }

    public void SetSex(string newSex)
    {
        sex = newSex;
    }

    public void SetWeight(string weightStr)
    {
        if (float.TryParse(weightStr, out float newWeight))
        {
            weight = newWeight;
        }
    }

    public void SetPAL(string palStr)
    {
        if (float.TryParse(palStr, out float newPAL))
        {
            physicalActivityLevel = newPAL;
        }
    }
}