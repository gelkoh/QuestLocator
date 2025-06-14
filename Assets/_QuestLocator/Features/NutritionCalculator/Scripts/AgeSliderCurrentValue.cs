using UnityEngine;
using TMPro;

public class AgeSliderCurrentValue : MonoBehaviour
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