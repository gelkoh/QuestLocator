using UnityEngine;
using TMPro;
using static NutritionCalculator;

public class AgeSliderCurrentValue : MonoBehaviour
{
    private TextMeshProUGUI _sliderCurrentValueDisplayText;

    void Start()
    {
        if (_sliderCurrentValueDisplayText == null)
        {
            _sliderCurrentValueDisplayText = gameObject.GetComponent<TextMeshProUGUI>();

            if (_sliderCurrentValueDisplayText != null)
            {
                _sliderCurrentValueDisplayText.SetText(NutritionCalculatorInstance.CurrentAge.ToString());
            }
            else
            {
                Debug.LogWarning("SliderCurrentValueDisplayAsInt: Couldn't get TextMeshProUGUI component on this element.");
            }
        }
    }

    public void OnSliderValueChanged(float newValue)
    {
        if (newValue == 65)
        {
            _sliderCurrentValueDisplayText.SetText($"{newValue:0}+");
        }
        else
        {
            _sliderCurrentValueDisplayText.SetText($"{newValue:0}");
        }
    }
}