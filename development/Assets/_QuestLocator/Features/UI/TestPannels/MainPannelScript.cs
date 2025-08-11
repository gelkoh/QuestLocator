using UnityEngine;
using TMPro;

public class MainPannelScript : MonoBehaviour
{
    private ProductParent productDisplayScript;
    [SerializeField] TextMeshProUGUI productName;

    void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        FillInfo();
    }

    private void FillInfo()
    {
        productName.text = productDisplayScript.productData.Product.ProductName;
    }
}