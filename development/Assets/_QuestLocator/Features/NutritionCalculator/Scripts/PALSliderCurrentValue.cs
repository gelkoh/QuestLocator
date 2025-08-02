using UnityEngine;
using TMPro;
using static NutritionCalculator;

public class PALSliderCurrentValue : MonoBehaviour
{
    private TextMeshProUGUI _sliderCurrentValueDisplayText;

    void Start()
    {
        if (_sliderCurrentValueDisplayText == null)
        {
            _sliderCurrentValueDisplayText = gameObject.GetComponent<TextMeshProUGUI>();

            if (_sliderCurrentValueDisplayText != null)
            {
                _sliderCurrentValueDisplayText.SetText(NutritionCalculatorInstance.CurrentPhysicalActivityLevel.ToString());

                Debug.LogWarning("[PalSliderCurrentValue] NutritionCalculatorInstance.CurrentPhysicalActivityLevel.ToString()");
            }
            else
            {
                Debug.LogWarning("SliderCurrentValueDisplayAsInt: Couldn't get TextMeshProUGUI component on this element.");
            }
        }
    }

    public void OnSliderValueChanged(float newValue)
    {
        float displayedValue = newValue / 10f;
        _sliderCurrentValueDisplayText.SetText($"{displayedValue:F1}");
    }
}