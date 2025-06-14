using UnityEngine;
using TMPro;

public class PALSliderCurrentValue : MonoBehaviour
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
        float displayedValue = newValue / 10f;
        _sliderCurrentValueDisplayText.SetText($"{displayedValue:F1}");
    }
}