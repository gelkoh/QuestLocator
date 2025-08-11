using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ScanHistoryManager : MonoBehaviour
{
    // Singleton
    public static ScanHistoryManager ScanHistoryManagerInstance { get; private set; }
    public event Action OnHistoryChanged;
    [SerializeField] private int _maxSavedProductsAmount = 100;
    private SavedProductsData _savedProductsData = new SavedProductsData();
    private string _saveFilePath = "";

    void Awake()
    {
        if (ScanHistoryManagerInstance == null)
        {
            ScanHistoryManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _saveFilePath = Path.Combine(Application.persistentDataPath, "scanned-products.json");
        Debug.Log($"ScanHistoryManager: Save File Path: {_saveFilePath}");
        LoadProducts();
    }

    public void AddProductAndSave(Root productRoot)
    {
        _savedProductsData.ProductsRoots.Insert(0, productRoot);

        if (_savedProductsData.ProductsRoots.Count > _maxSavedProductsAmount)
        {
            _savedProductsData.ProductsRoots.RemoveAt(_savedProductsData.ProductsRoots.Count - 1);
        }

        SaveProducts();
        OnHistoryChanged?.Invoke();
        Debug.Log("ProductHistoryManager: Added a product and saved.");
    }

    public void RemoveProductAndSave(Root productRoot)
    {
        _savedProductsData.ProductsRoots.Remove(productRoot);
        SaveProducts();
        Debug.Log("ProductHistoryManager: Removed a product and saved.");
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

                Debug.Log($"ProductHistoryManager: {_savedProductsData.ProductsRoots.Count} products loaded from " + _saveFilePath);
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

    public List<Root> GetSavedProducts()
    {
        return _savedProductsData.ProductsRoots;
    }

    public void ClearSavedProducts()
    {
        _savedProductsData = new SavedProductsData();
        SaveProducts();
        Debug.Log("ProductHistoryManager: All saved products cleared.");
    }
}