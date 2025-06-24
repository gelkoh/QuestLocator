using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;

public class NutritionCalculator : MonoBehaviour
{
    public static NutritionCalculator NutritionCalculatorInstance { get; private set; }
    public Action<NutritionRecommendation> OnNutritionRecommendationCalculated;

    [Header("Nutrition Data CSV")]
    [SerializeField] private TextAsset _nutritionDataCSV;

    [Header("Default values (before first save)")]
    [SerializeField] private string _sex = "Männlich";
    [SerializeField] private int _age = 25;
    [SerializeField] private int _weight = 70;
    [SerializeField] private float _physicalActivityLevel = 1.8f;

    public int CurrentAge => _age;
    public string CurrentSex => _sex;
    public int CurrentWeight => _weight;
    public float CurrentPhysicalActivityLevel => _physicalActivityLevel;

    private readonly List<NutritionData> nutritionDataList = new();
    
    private SavedBodyMetrics _savedBodyMetrics = new();
    private string _saveFilePath = "";

    private Coroutine _saveCoroutine;
    private const float SAVE_DELAY_SECONDS = 3.0f;
    private NutritionRecommendation _currentNutritionRecommendation;

    public NutritionRecommendation CurrentNutritionRecommendation => _currentNutritionRecommendation;

    void Awake()
    {
        if (NutritionCalculatorInstance == null)
        {
            NutritionCalculatorInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _saveFilePath = Path.Combine(Application.persistentDataPath, "saved-body-metrics.json");
        
        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath);
                _savedBodyMetrics = JsonConvert.DeserializeObject<SavedBodyMetrics>(json);

                if (_savedBodyMetrics == null)
                {
                    Debug.LogWarning("[NutritionCalculator] Loaded body metrics were null (JSON parsing error or empty file). Initializing with default values.");
                    _savedBodyMetrics = new();
                }
                else
                {
                    _sex = _savedBodyMetrics.Sex;
                    _age = _savedBodyMetrics.Age;
                    _weight = _savedBodyMetrics.Weight;
                    _physicalActivityLevel = _savedBodyMetrics.PhysicalActivityLevel;
                    Debug.Log("[NutritionCalculator] Body metrics loaded successfully.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NutritionCalculator] Error loading body metrics: {ex.Message}. Initializing with default values.");
            }
        }
        else
        {
            Debug.Log("[NutritionCalculator] No saved body metrics file found. Initializing with default values.");
            SaveBodyMetricsInstant();
        }

        LoadNutritionData();
        UpdateNutritionRecommendation();
    }

    private void LoadNutritionData()
    {
        if (_nutritionDataCSV == null)
        {
            Debug.LogError("[NutritionCalculator] No CSV assigned.");
            return;
        }

        string[] lines = _nutritionDataCSV.text.Split('\n');
        Debug.Log($"[NutritionCalculator] CSV-lines loaded: {lines.Length}");

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

            nutritionDataList.Add(data);
        }
    }

    private float ParseFloat(string value)
    {
        value = value.Replace(",", ".");
        return float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result) ? result : 0f;
    }

    public void OnSexDropdownValueChanged(int newIndex)
    {
        _sex = newIndex switch
        {
            0 => "Männlich",
            1 => "Weiblich",
            _ => throw new NotImplementedException()
        };
        
        UpdateAndScheduleSave();
    }

    public void OnAgeSliderValueChanged(float age)
    {
        _age = (int)age;
        UpdateAndScheduleSave();
    }

    public void OnWeightSliderValueChanged(float weight)
    {
        _weight = (int)weight;
        UpdateAndScheduleSave();
    }

    public void OnPhysicalActivityLevelValueChanged(float pal)
    {
        _physicalActivityLevel = pal / 10f; 
        UpdateAndScheduleSave();
    }

    private void UpdateAndScheduleSave()
    {
        UpdateNutritionRecommendation();
        
        if (_saveCoroutine != null)
        {
            StopCoroutine(_saveCoroutine);
        }

        _saveCoroutine = StartCoroutine(DelayedSaveCoroutine());
    }

    private IEnumerator DelayedSaveCoroutine()
    {
        yield return new WaitForSeconds(SAVE_DELAY_SECONDS);
        SaveBodyMetricsInstant();
        _saveCoroutine = null;
    }

    public void UpdateNutritionRecommendation()
    {
        Debug.Log("[NutritionCalculator] Updating nutrition recommendation.");
        NutritionData matchedData = FindMatchingData();

        if (matchedData != null)
        {
            NutritionRecommendation recommendation = CalculateRecommendation(matchedData);
            Debug.Log($"[Daily need calculated – kcal: {recommendation.energyKcal}, Protein: {recommendation.proteinG}, Fat: {recommendation.fatG}, Saturated Fat: {recommendation.satFatG}, Sugar: {recommendation.sugarG}, Carbs: {recommendation.carbsG}");
            OnNutritionRecommendationCalculated?.Invoke(recommendation);
        }
        else
        {
            Debug.LogWarning("[NutritionCalculator] Problem updating nutrition recommendation.");
        }
    }

    private NutritionData FindMatchingData()
    {
        foreach (var data in nutritionDataList)
        {
            if (IsInAgeGroup(data.ageGroup) && data.sex == _sex && Mathf.Approximately(data.pal, _physicalActivityLevel))
            {
                Debug.Log("[NutritionCalculator] Found matching data.");
                return data;
            }
        }

        foreach (var data in nutritionDataList)
        {
            if (IsInAgeGroup(data.ageGroup) && data.sex == _sex)
            {
                Debug.Log("[NutritionCalculator] Found matching data (PAL divergent).");
                return data;
            }
        }

        var fallback = nutritionDataList.Find(d =>
            d.ageGroup.Trim().Equals("Default", StringComparison.OrdinalIgnoreCase)
        );

        if (fallback != null)
        {
            Debug.LogWarning("[NutritionCalculator] Couldn't find an exact match. Fallback to default values.");
            return fallback;
        }

        Debug.LogError("[NutritionCalculator] Default values not found.");
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

        Debug.LogWarning($"[NutritionCalculator] '{ageGroup}' couldn't be parsed.");
        return false;
    }

    private NutritionRecommendation CalculateRecommendation(NutritionData data)
    {
        float palAdjustment = _physicalActivityLevel / data.pal;

        NutritionRecommendation nutritionRecommendation = new NutritionRecommendation
        {
            energyKcal = data.energyKcal * palAdjustment,
            proteinG = data.proteinGPerKg * _weight,
            fatG = data.fatG * palAdjustment,
            satFatG = data.satFatG * palAdjustment,
            carbsG = data.carbsG * palAdjustment,
            sugarG = data.sugarG * palAdjustment
        };

        _currentNutritionRecommendation = nutritionRecommendation;
        return nutritionRecommendation;
    }

    private void SaveBodyMetricsInstant()
    {
        try
        {
            _savedBodyMetrics.Sex = _sex;
            _savedBodyMetrics.Age = _age;
            _savedBodyMetrics.Weight = _weight;
            _savedBodyMetrics.PhysicalActivityLevel = _physicalActivityLevel;

            string json = JsonConvert.SerializeObject(_savedBodyMetrics, Formatting.Indented);
            File.WriteAllText(_saveFilePath, json);
            Debug.Log("[NutritionCalculator] Body metrics saved to " + _saveFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[NutritionCalculator] Error saving body metrics: {ex.Message}");
        }
    }

    // private void ClearSavedBodyMetrics()
    // {
    //     if (File.Exists(_saveFilePath))
    //     {
    //         File.Delete(_saveFilePath);
    //         Debug.Log("[NutritionCalculator] Saved body metrics file deleted.");
    //     }
       
    //     _sex = "Männlich";
    //     _age = 25;
    //     _weight = 70;
    //     _physicalActivityLevel = 1.8f;

    //     SaveBodyMetricsInstant();

    //     Debug.Log("[NutritionCalculator] Body metrics cleared and reset to defaults.");
    //     UpdateNutritionRecommendation();
    // }
}