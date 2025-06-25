using System;
using UnityEngine;
using UnityEngine.UI;
using static NutritionCalculator;

public class SexDropdown : MonoBehaviour
{
    private Dropdown _sexDropdown;

    void Start()
    {
        String currentSex = NutritionCalculatorInstance.CurrentSex;
        int dropdownValue;

        if (currentSex == "Männlich")
        {
            dropdownValue = 0;
        }
        else
        {
            dropdownValue = 1;
        }

        if (_sexDropdown == null)
        {
            _sexDropdown = gameObject.GetComponent<Dropdown>();

            if (_sexDropdown == null)
            {
                Debug.LogWarning("[SexDropdown] Couldn't get Dropdown component on this element.");
                return;  
            }
            
            _sexDropdown.value = dropdownValue;
        }
    }
}