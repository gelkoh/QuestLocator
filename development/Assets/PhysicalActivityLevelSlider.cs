using UnityEngine;
using UnityEngine.UI;
using static NutritionCalculator;

public class PhysicalActivityLevelSlider : MonoBehaviour
{
    private Slider _physicalActivityLevelSlider;

    void Start()
    {
        if (_physicalActivityLevelSlider == null)
        {
            _physicalActivityLevelSlider = gameObject.GetComponent<Slider>();

            if (_physicalActivityLevelSlider == null)
            {
                Debug.LogWarning("[PhysicalActivitySlider] Couldn't get Slider component on this element.");
                return;  
            }
            
            _physicalActivityLevelSlider.value = NutritionCalculatorInstance.CurrentPhysicalActivityLevel * 10;
        }
    }
}
