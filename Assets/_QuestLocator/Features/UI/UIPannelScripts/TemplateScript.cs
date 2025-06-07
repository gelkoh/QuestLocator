using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TemplateScript : MonoBehaviour
{
    private ProductParent productDisplayScript;
    
    [SerializeField] TextMeshProUGUI PanelProduct;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PanelProduct.text = productDisplayScript.productData.Product.ProductName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
