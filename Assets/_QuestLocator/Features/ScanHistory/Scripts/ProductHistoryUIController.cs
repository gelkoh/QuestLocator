using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ProductHistoryManager;
using static BarcodeProcessor;

public class ProductHistoryUIController : MonoBehaviour
{
    [SerializeField] private Transform _historyItemsParent;
    [SerializeField] private GameObject _historyItemPrefab;

    void Awake()
    {
        BarcodeProcessorInstance.OnProductProcessed += HandleProductProcessed;
    }

    void Start()
    {
        List<Product> products = ProductHistoryManagerInstance.GetSavedProducts();

        products.ForEach(product =>
        {
            AddProductToPanel(product);
        });
    }

    private void HandleProductProcessed(bool success, string productNameOrError, Root productRoot)
    {
        if (success)
            AddProductToPanel(productRoot.Product);
    }

    private void AddProductToPanel(Product product)
    {
        GameObject newProductGO = Instantiate(_historyItemPrefab, _historyItemsParent);
        TextMeshProUGUI productName = newProductGO.GetComponent<TextMeshProUGUI>();
        productName.SetText(product.ProductName);
    }
}