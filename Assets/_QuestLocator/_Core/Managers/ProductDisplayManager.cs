using System;
using UnityEngine;
using static BarcodeProcessor;
using static NutritionCalculator;

public class ProductDisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform productPrefabParent;

    [Header("Product Display Positioning")]
    [SerializeField] private float distanceFromCamera = 2.0f;
    [SerializeField] private Vector3 displayOffset = new Vector3(0, 0, 0);

    private Camera mainCamera;

    // Singleton
    public static ProductDisplayManager ProductDisplayManagerInstance { get; private set; }

    private void Awake()
    {
        if (ProductDisplayManagerInstance == null)
        {
            ProductDisplayManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found! Please tag your main camera as 'MainCamera'.");
        }
    }

    private void OnEnable()
    {
        if (BarcodeProcessorInstance != null)
        {
            BarcodeProcessorInstance.OnProductProcessed += HandleProductProcessed;
            Debug.Log("ProductDisplayManager subscribed to BarcodeProcessor.OnProductProcessed.");
        }
        else
        {
            Debug.LogWarning("BarcodeProcessor.Instance not found. ProductDisplayManager cannot subscribe.");
        }
    }

    private void OnDisable()
    {
        if (BarcodeProcessorInstance != null)
        {
            BarcodeProcessorInstance.OnProductProcessed -= HandleProductProcessed;
        }
    }

    private void HandleProductProcessed(bool success, string productNameOrError, Root productRoot)
    {
        if (success)
        {
            Debug.Log($"Displaying product: {productNameOrError}");
            InstantiateAndFillProductPrefab(productRoot);
        }
        else
        {
            Debug.LogWarning($"Could not display product (API error or not found): {productNameOrError}");
        }
    }

    public void InstantiateAndFillProductPrefab(Root productRoot)
    {
        Debug.LogError("inInstancingAndFill---");

        if (productPrefab == null)
        {
            Debug.LogError("Product Prefab is not assigned in the Inspector of ProductDisplayManager!");
            return;
        }

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

        Vector3 euler = mainCamera.transform.rotation.eulerAngles;
        euler.z = 0;
        spawnRotation = Quaternion.Euler(euler);
        try
        {
            GameObject newProductGO = Instantiate(productPrefab, spawnPosition, spawnRotation, productPrefabParent);
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
                NutrientsPannelScript panelScript = newProductGO.GetComponentInChildren<NutrientsPannelScript>();

                if (panelScript != null)
                {
                    panelScript.InitAfterDataAvailable(NutritionCalculatorInstance.CurrentNutritionRecommendation);
                }
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
}