using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TemplateScript : MonoBehaviour
{
    private ProductParent productDisplayScript;
    [SerializeField] TextMeshProUGUI PanelType;
    [SerializeField] TextMeshProUGUI PanelProduct;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        ColliderResizer childScript = GetComponentInChildren<ColliderResizer>();
        PanelType.text = "Test";
        PanelProduct.text = productDisplayScript.productData.Product.ProductName;
        //childScript.UpdateCollider();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
