using UnityEngine;
using UnityEngine.UI;
using static NutritionCalculator;

public class WeightSlider : MonoBehaviour
{
    private Slider _weightSlider;

    void Start()
    {
        if (_weightSlider == null)
        {
            _weightSlider = gameObject.GetComponent<Slider>();

            if (_weightSlider == null)
            {
                Debug.LogWarning("[WeightSlider] Couldn't get Slider component on this element.");
                return;
            }

            _weightSlider.value = NutritionCalculatorInstance.CurrentWeight;
        }
    }
}
