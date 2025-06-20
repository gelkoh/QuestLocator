using UnityEngine;
using TMPro;
using System.Collections;

public class NutrientsPannelScript : MonoBehaviour
{
    //[Header("Manuelle EAN Eingabe")]
    //[SerializeField] private string ean = "3017620422003"; // z. B. Nutella
    [SerializeField] private NutrientBarFiller nutrientBarFiller;
    [SerializeField] private TextMeshProUGUI productName;

    private Root productData;

    private ProductParent productDisplayScript;

    private void Start()
    {
        //StartCoroutine(OpenFoodFactsClientCall());
        productDisplayScript = GetComponentInParent<ProductParent>();
        TryFillBars();
    }

    /* private IEnumerator OpenFoodFactsClientCall()
    {
        var client = new OpenFoodFactsClient();
        yield return client.GetProductByEan(ean,
            onSuccess: (root) =>
            {
                if (root?.Product == null)
                {
                    Debug.LogError("❌ Produktdaten fehlen – EAN ungültig?");
                    return;
                }

                if (root.Product.Nutriments == null)
                {
                    Debug.LogError("❌ Nährwertdaten fehlen – Produkt ist in der API nicht vollständig.");
                    return;
                }

                productData = root;
                productName.text = root.Product.ProductName;

                TryFillBars();
            },
            onError: (err) =>
            {
                Debug.LogError("API Fehler: " + err);
            });
    } */

    private void TryFillBars()
    {
        productName.text = productDisplayScript.productData.Product.ProductName;
        if (nutrientBarFiller == null)
        {
            Debug.LogError("❌ nutrientBarFiller ist nicht zugewiesen! Bitte das GameObject mit dem Script (z. B. col1) im Inspector eintragen.");
            return;
        }

        /* if (productData?.Product?.Nutriments == null)
        {
            Debug.LogWarning("⚠️ Keine Nährwertdaten verfügbar.");
            return;
        } */
        if (productDisplayScript.productData.Product.Nutriments == null)
        {
            Debug.LogWarning("⚠️ Keine Nährwertdaten verfügbar.");
            return;
        }

        Debug.Log("📊 FillBars() wird aufgerufen...");
    
        var calc = NutritionCalculator.NutritionCalculatorInstance;

        // Standardwerte setzen (damit Berechnung funktioniert)
        calc.OnAgeSliderValueChanged(calc.defaultAge);
        calc.OnSexDropdownValueChanged(1); // 1 = Männlich
        calc.OnWeightSliderValueChanged(calc.defaultWeight);
        calc.OnPALSliderValueChanged(calc.defaultPAL * 10f); // Slider erwartet *10

        // Event-Callback registrieren
        calc.OnNutritionRecommendationCalculated = null;
        calc.OnNutritionRecommendationCalculated += (recommendation) =>
        {
            //Debug.Log($"➡️ FillBars gestartet Energie: {productData.Product.Nutriments?.EnergyKcal100G}, Empfohlen: {recommendation?.energyKcal}");
            nutrientBarFiller.FillBars(productDisplayScript.productData.Product.Nutriments, recommendation);
        };

        // Falls nicht schon intern berechnet wurde
        calc.UpdateNutritionRecommendation();
    }
}
