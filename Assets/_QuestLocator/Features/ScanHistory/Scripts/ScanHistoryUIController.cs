using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScanHistoryManager;

public class ProductHistoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject _productPrefab;
    [SerializeField] private GameObject _scanHistoryPanel;
    [SerializeField] private Transform _historyItemsParent;
    [SerializeField] private GameObject _historyItemPrefab;
    [SerializeField] private Vector3 displayOffset = new Vector3(0, 0, 0);
    [SerializeField] private float distanceFromCamera = 0.5f;

    private Camera mainCamera;

    void OnEnable()
    {
        ScanHistoryManagerInstance.OnHistoryChanged += HandleHistoryChanged;
        _scanHistoryPanel.SetActive(false);
    }

    void OnDisable()
    {
        ScanHistoryManagerInstance.OnHistoryChanged -= HandleHistoryChanged;
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found! Please tag your main camera as 'MainCamera'.");
        }
    }

    void Start()
    {
        UpdateUI();
    }

    private void AddProductToPanel(Root productProduct, int idx)
    {
        GameObject newProductGO = Instantiate(_historyItemPrefab, _historyItemsParent);

        Transform indexTransform = newProductGO.transform.Find("ScanHistoryItemIndex");
        TextMeshProUGUI index = indexTransform.GetComponent<TextMeshProUGUI>();
        index.SetText(idx.ToString());

        Transform productNameTransform = newProductGO.transform.Find("ScanHistoryItemProductName");
        TextMeshProUGUI productName = productNameTransform.GetComponent<TextMeshProUGUI>();
        productName.SetText(productProduct.Product.ProductName);

        Transform viewButtonTransform = newProductGO.transform.Find("ScanHistoryItemViewButton");
        Button viewButton = viewButtonTransform.GetComponent<Button>();
        viewButton.onClick.RemoveAllListeners();
        viewButton.onClick.AddListener(() => HandleViewButtonClick(productProduct));

        Transform removeButtonTransform = newProductGO.transform.Find("ScanHistoryItemRemoveButton");
        Button removeButton = removeButtonTransform.GetComponent<Button>();
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => HandleRemoveButtonClick(productProduct));

        Transform clearAllButtonTransform = newProductGO.transform.Find("ClearAllButton");
        Button clearAllButton = clearAllButtonTransform.GetComponent<Button>();
        clearAllButton.onClick.RemoveAllListeners();
        clearAllButton.onClick.AddListener(HandleClearAllButtonClick);
    }

    private void HandleHistoryChanged()
    {
        UpdateUI();
    }

    private void HandleViewButtonClick(Root productRoot)
    {
        InstantiateProduct(productRoot);
    }

    private void HandleRemoveButtonClick(Root productRoot)
    {
        ScanHistoryManagerInstance.RemoveProductAndSave(productRoot);
        UpdateUI();
    }

    private void HandleClearAllButtonClick()
    {
        ScanHistoryManagerInstance.ClearSavedProducts();
        UpdateUI();
    }

    private void UpdateUI()
    {
        RemoveAllItems();

        List<Root> productsRoots = ScanHistoryManagerInstance.GetSavedProducts();

        for (int i = 0; i < productsRoots.Count; i++)
        {
            AddProductToPanel(productsRoots[i], i + 1);
        }
    }

    private void RemoveAllItems()
    {
        foreach (Transform child in _historyItemsParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void InstantiateProduct(Root productRoot)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is null! Cannot spawn product prefab.");
            return;
        }

        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

        spawnPosition += mainCamera.transform.right * displayOffset.x;
        spawnPosition += mainCamera.transform.up * displayOffset.y;
        spawnPosition += mainCamera.transform.forward * displayOffset.z;

        Quaternion spawnRotation = Quaternion.LookRotation(mainCamera.transform.position - spawnPosition);

        spawnRotation = mainCamera.transform.rotation;
        try
        {
            GameObject newProductGO = Instantiate(_productPrefab, spawnPosition, spawnRotation, transform);
            if (productRoot != null && productRoot.Product != null && !string.IsNullOrEmpty(productRoot.Product.ProductName))
            {
                newProductGO.name = $"ProductDisplay_{productRoot.Product.ProductName.Replace(" ", "_").Replace("/", "_")}";
            }
            else
            {
                newProductGO.name = $"ProductDisplay_Unknown";
            }

            ProductParent productDisplayScript = newProductGO.GetComponent<ProductParent>();


            if (productDisplayScript != null)
            {
                productDisplayScript.SetProductData(productRoot);
            }
            else
            {
                Debug.LogWarning("Product Prefab does not have a 'ProductDisplay' script attached!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Instancing Parent::" + ex.Message);
        }
    }

    public void ToggleScanHistoryPanelVisible()
    {
        if (_scanHistoryPanel.activeSelf)
        {
            Debug.Log("ScanHistoryUIController: Hide scan history panel");
            _scanHistoryPanel.SetActive(false);
        }
        else
        {
            Debug.Log("ScanHistoryUIController: Show scan history panel");
            _scanHistoryPanel.SetActive(true);
        }
    }
}