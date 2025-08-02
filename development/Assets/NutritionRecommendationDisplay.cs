using UnityEngine;
using TMPro;
using static NutritionCalculator;

public class NutritionRecommendationDisplay : MonoBehaviour
{
    private TextMeshProUGUI _nutritionRecommendationDisplayText;

    void Awake()
    {
        if (_nutritionRecommendationDisplayText == null)
        {
            _nutritionRecommendationDisplayText = gameObject.GetComponent<TextMeshProUGUI>();

            if (_nutritionRecommendationDisplayText == null)
            {
                Debug.LogWarning("NutritionRecommendationDisplay: Could not find TextMeshProUGUI component on this element.");
            }
        }
    }

    void OnEnable()
    {
        NutritionCalculatorInstance.OnNutritionRecommendationCalculated += DisplayNutritionRecommendation;
    }

    void OnDisable()
    {
        NutritionCalculatorInstance.OnNutritionRecommendationCalculated -= DisplayNutritionRecommendation;
    }

    private void DisplayNutritionRecommendation(NutritionRecommendation nutritionRecommendation)
    {
        _nutritionRecommendationDisplayText.SetText(@$"Energy: {nutritionRecommendation.energyKcal:0} kcal
Protein: {nutritionRecommendation.proteinG:0} g
Fat: {nutritionRecommendation.fatG:0} g
Sat fat: {nutritionRecommendation.satFatG:0} g
Carbs: {nutritionRecommendation.carbsG:0} g
Sugar: {nutritionRecommendation.sugarG:0} g");
    }
}
