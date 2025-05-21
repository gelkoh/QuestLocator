using UnityEngine;

public class ProductDisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject productPrefab;
    
    [Header("Product Display Positioning")]
    [SerializeField] private float distanceFromCamera = 2.0f; 
    [SerializeField] private Vector3 displayOffset = new Vector3(0, 0, 0);

    private Camera mainCamera;

    /*private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found! Please tag your main camera as 'MainCamera'.");
        }
    }

    private void OnEnable()
    {
        if (BarcodeProcessor.Instance != null)
        {
            BarcodeProcessor.Instance.OnProductProcessed += HandleProductProcessed;
            Debug.Log("ProductDisplayManager subscribed to BarcodeProcessor.OnProductProcessed.");
        }
        else
        {
            Debug.LogWarning("BarcodeProcessor.Instance not found. ProductDisplayManager cannot subscribe.");
        }
    }

    private void OnDisable()
    {
        if (BarcodeProcessor.Instance != null)
        {
            BarcodeProcessor.Instance.OnProductProcessed -= HandleProductProcessed;
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

    private void InstantiateAndFillProductPrefab(Root productRoot)
    {
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
       
        spawnRotation = mainCamera.transform.rotation;

        GameObject newProductGO = Instantiate(productPrefab, spawnPosition, spawnRotation);
        
        if (productRoot != null && productRoot.product != null && !string.IsNullOrEmpty(productRoot.product.product_name))
        {
            newProductGO.name = $"ProductDisplay_{productRoot.product.product_name.Replace(" ", "_").Replace("/", "_")}";
        }
        else
        {
            newProductGO.name = $"ProductDisplay_Unknown";
        }

        ProductDisplay productDisplayScript = newProductGO.GetComponent<ProductDisplay>();

        if (productDisplayScript != null)
        {
            productDisplayScript.SetProductData(productRoot);
        }
        else
        {
            Debug.LogWarning("Product Prefab does not have a 'ProductDisplay' script attached!");
        }
    }*/
}