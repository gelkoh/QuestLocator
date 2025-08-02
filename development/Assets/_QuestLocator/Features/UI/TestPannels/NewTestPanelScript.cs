using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewTestPanelScript : MonoBehaviour
{
    private ProductParent productDisplayScript;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject IngredientsSection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        FillInfo();
    }

    // Update is called once per frame
    private void FillInfo()
    {
        title.text = productDisplayScript.productData.Product.ProductName;

        foreach (var ingredient in productDisplayScript.productData.Product.Ingredients)
        {
            GameObject textObj = new GameObject("IngredientText");
            textObj.transform.SetParent(IngredientsSection.transform, false); // 'false' keeps local scale

            // Add TextMeshProUGUI component
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = ingredient.Text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.enableAutoSizing = true;

            // Optionally, add LayoutElement if you want to control spacing/size
            var layout = textObj.AddComponent<LayoutElement>();
            layout.preferredHeight = 30;
        }
    }
}