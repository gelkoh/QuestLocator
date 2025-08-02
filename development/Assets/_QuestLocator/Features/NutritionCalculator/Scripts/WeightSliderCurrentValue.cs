using UnityEngine;
using TMPro;
using static NutritionCalculator;

public class WeightSliderCurrentValue : MonoBehaviour
{
    private TextMeshProUGUI _sliderCurrentValueDisplayText;

    void Start()
    {
        if (_sliderCurrentValueDisplayText == null)
        {
            _sliderCurrentValueDisplayText = gameObject.GetComponent<TextMeshProUGUI>();

            if (_sliderCurrentValueDisplayText != null)
            {
                _sliderCurrentValueDisplayText.SetText(NutritionCalculatorInstance.CurrentWeight.ToString());
            }
            else
            {
                Debug.LogWarning("SliderCurrentValueDisplayAsInt: Couldn't get TextMeshProUGUI component on this element.");
            }
        }
    }

    public void OnSliderValueChanged(float newValue)
    {
        _sliderCurrentValueDisplayText.SetText($"{newValue:0}");
    }
}