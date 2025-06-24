using UnityEngine;
using UnityEngine.UI;
using static NutritionCalculator;

public class AgeSlider : MonoBehaviour
{
    private Slider _ageSlider;

    void Start()
    {
        if (_ageSlider == null)
        {
            _ageSlider = gameObject.GetComponent<Slider>();

            if (_ageSlider == null)
            {
                Debug.LogWarning("[PhysicalActivitySlider] Couldn't get Slider component on this element.");
                return;
            }

            _ageSlider.value = NutritionCalculatorInstance.CurrentAge;
        }
    }
}
