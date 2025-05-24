using TMPro;
using UnityEngine;

public class ProductDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _productNameTextMeshPro;

    public void SetProductData(Root root)
    {
        Debug.LogError("Setting product data");
        _productNameTextMeshPro.text = root.product.product_name;

        // if (_productNameTextMeshPro == null)
        // {
        //     Debug.LogError("productNameText is not assigned in the inspector.");
        //     return;
        // }
        
        // if (!gameObject.activeInHierarchy)
        // {
        //     Debug.LogWarning("The GameObject containing ProductInfoPanel is not active.");
        // }

        // Debug.Log("Setting product name to: " + name);
        // _productNameTextMeshPro.text = name;

        // if (_productNameTextMeshPro.text != name)
        // {
        //     Debug.LogError("Failed to set the product name text. Check for TMP issues.");
        // }
    }
}
