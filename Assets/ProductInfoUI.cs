using TMPro;
using UnityEngine;

public class ProductInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _productNameTextMeshPro;

    public void SetData(string name)
    {
        if (_productNameTextMeshPro == null)
        {
            Debug.LogError("productNameText is not assigned in the inspector.");
            return;
        }
        
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("The GameObject containing ProductInfoPanel is not active.");
        }

        Debug.Log("Setting product name to: " + name);
        _productNameTextMeshPro.text = name;

        if (_productNameTextMeshPro.text != name)
        {
            Debug.LogError("Failed to set the product name text. Check for TMP issues.");
        }
    }
}
