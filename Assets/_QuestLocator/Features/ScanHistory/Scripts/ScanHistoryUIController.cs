using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScanHistoryManager;
using static ProductDisplayManager;
using Oculus.Interaction;

public class ProductHistoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject _scanHistoryPanel;
    [SerializeField] private Transform _historyItemsParent;
    [SerializeField] private GameObject _historyItemPrefab;
    [SerializeField] private Button _clearAllButton;

    [SerializeField] private UIThemeManagerLocal _themeManager;

    private Camera mainCamera;
    private PanelPositioner _scanHistoryPanelPositioner;

    void OnEnable()
    {
        if (_themeManager != null)
        {
            _themeManager.ApplyTheme(_themeManager.CurrentThemeIndex);
            Debug.Log($"[ScanHistoryPanelThemer] Applied theme to '{gameObject.name}' on OnEnable.");
        }
        else
        {
            Debug.LogError($"[ScanHistoryPanelThemer] UIThemeManagerLocal not assigned for {gameObject.name}!");
        }

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

        if (_clearAllButton != null)
        {
            _clearAllButton.onClick.RemoveAllListeners();
            _clearAllButton.onClick.AddListener(HandleClearAllButtonClick);
        }
        else
        {
            Debug.LogError("ScanHistoryUIController: Clear button is not assigned in the inspector.");
        }

        _scanHistoryPanelPositioner = _scanHistoryPanel.GetComponentInChildren<Canvas>().GetComponent<PanelPositioner>();
    }

    void Start()
    {
        UpdateUI();
    }

    private void AddProductToPanel(Root productRoot, int idx)
    {
        GameObject newProductGO = Instantiate(_historyItemPrefab, _historyItemsParent);

        Transform indexTransform = newProductGO.transform.Find("ScanHistoryItemIndex");
        TextMeshProUGUI index = indexTransform.GetComponent<TextMeshProUGUI>();
        index.SetText(idx.ToString());

        Transform productNameTransform = newProductGO.transform.Find("ScanHistoryItemProductName");
        TextMeshProUGUI productName = productNameTransform.GetComponent<TextMeshProUGUI>();
        productName.SetText(productRoot.Product.ProductName);

        Transform viewButtonTransform = newProductGO.transform.Find("ScanHistoryItemViewButton");
        Button viewButton = viewButtonTransform.GetComponent<Button>();
        viewButton.onClick.RemoveAllListeners();
        viewButton.onClick.AddListener(() => HandleViewButtonClick(productRoot));

        Transform removeButtonTransform = newProductGO.transform.Find("ScanHistoryItemRemoveButton");
        Button removeButton = removeButtonTransform.GetComponent<Button>();
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => HandleRemoveButtonClick(productRoot));

        Debug.Log("ScanHistoryUIController: Inside end of addproducttopanel()");
    }

    private void HandleHistoryChanged()
    {
        Debug.Log("ScanHistoryUIController: Handle History Changed");
        UpdateUI();
    }

    private void HandleViewButtonClick(Root productRoot)
    {
        Debug.Log("ScanHistoryUIController: Handling view button click");
        ProductDisplayManagerInstance.InstantiateAndFillProductPrefab(productRoot);
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
        Debug.Log("ScanHistoryUIController: Updating UI");
        RemoveAllItems();

        List<Root> productsRoots = ScanHistoryManagerInstance.GetSavedProducts();

        for (int i = 0; i < productsRoots.Count; i++)
        {
            AddProductToPanel(productsRoots[i], i + 1);
            Debug.Log("ScanHistoryUIController: Added product to panel.");
        }
    }

    private void RemoveAllItems()
    {
        foreach (Transform child in _historyItemsParent)
        {
            Destroy(child.gameObject);
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

            if (_scanHistoryPanelPositioner == null)
            {
                Debug.LogWarning("ScanHistoryUIController: Scan history panel positioner is null");
            }

            _scanHistoryPanelPositioner.PositionPanelInFrontOfCamera();
        }
    }
}