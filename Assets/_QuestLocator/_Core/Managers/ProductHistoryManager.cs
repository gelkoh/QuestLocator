using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ProductHistoryManager : MonoBehaviour
{
    // Singleton
    public static ProductHistoryManager ProductHistoryManagerInstance { get; private set; }

    [SerializeField] private int _maxSavedProductsAmount = 30;

    private SavedProductsData _savedProductsData = new SavedProductsData();

    private string _saveFilePath = "";

    void Awake()
    {
        if (ProductHistoryManagerInstance == null)
        {
            ProductHistoryManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _saveFilePath = Path.Combine(Application.persistentDataPath, "scanned-products.json");
        LoadProducts();   
    }

    public void AddProductAndSave(Product product)
    {
        _savedProductsData.Products.Insert(0, product);

        if (_savedProductsData.Products.Count > _maxSavedProductsAmount)
        {
            _savedProductsData.Products.RemoveAt(_savedProductsData.Products.Count - 1);
        }

        SaveProducts();
        Debug.Log("ProductHistoryManager: Added a product and saved.");
    }

    private void SaveProducts()
    {
        try
        {
            string json = JsonConvert.SerializeObject(_savedProductsData, Formatting.Indented);

            File.WriteAllText(_saveFilePath, json);
            Debug.Log("ProductHistoryManager: Products saved to " + _saveFilePath);
        }
        catch (Exception ex)
        {
            Debug.Log($"ProductHistoryManager: Error saving products: {ex.Message}");
        }
    }

    private void LoadProducts()
    {
        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath);

                _savedProductsData = JsonConvert.DeserializeObject<SavedProductsData>(json);

                if (_savedProductsData == null)
                {
                    _savedProductsData = new SavedProductsData();
                    Debug.LogWarning("ProductHistoryManager: Loaded data was null, initializing new SavedProductsData.");
                }

                Debug.Log($"ProductHistoryManager: {_savedProductsData.Products.Count} products loaded from " + _saveFilePath);
            }
            catch (Exception ex)
            {
                Debug.Log($"ProductHistoryManager: Error loading products: {ex.Message}. Starting with empty list");
                _savedProductsData = new SavedProductsData();
            }
        }
        else
        {
            Debug.Log("ProductHistoryManager: No saved products file found. Starting with empty list.");
            _savedProductsData = new SavedProductsData();
        }
    }

    public List<Product> GetSavedProducts()
    {
        return _savedProductsData.Products;
    }

    public void ClearSavedProducts()
    {
        _savedProductsData = new SavedProductsData();
        SaveProducts();
        Debug.Log("ProductHistoryManager: All saved products cleared.");
    }
}
