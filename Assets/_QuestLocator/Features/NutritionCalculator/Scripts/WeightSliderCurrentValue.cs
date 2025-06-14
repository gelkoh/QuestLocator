using UnityEngine;
using TMPro;

public class WeightSliderCurrentValue : MonoBehaviour
{
    private TextMeshProUGUI _sliderCurrentValueDisplayText;

    void Awake()
    {
        if (_sliderCurrentValueDisplayText == null)
        {
            _sliderCurrentValueDisplayText = gameObject.GetComponent<TextMeshProUGUI>();

            if (_sliderCurrentValueDisplayText)
            {
                Debug.LogWarning("SliderCurrentValueDisplayAsInt: Couldn't get TextMehsProUGUI component on this element.");
            }
        }
    }

    public void OnSliderValueChanged(float newValue)
    {
        _sliderCurrentValueDisplayText.SetText($"{newValue:0}");
    }
}