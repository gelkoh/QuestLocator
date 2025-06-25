using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;

public class ProductParent : MonoBehaviour
{
    public Root productData;

    [SerializeField] GameObject titlePanelPrefab;
    [SerializeField] Transform titleSpawn;
    GameObject titlePanelInstance;

    [SerializeField] GameObject gemininPanelPrefab;
    [SerializeField] Transform geminiSpawn;
    GameObject geminiPanelInstance = null;

    [SerializeField] GameObject ingredientPannelPrefab;
    [SerializeField] Transform ingredientsSpawn;
    GameObject ingredientPannelInstance = null;

    [SerializeField] GameObject nutritionPannelPrefab;
    [SerializeField] Transform nutritionSpawn;
    GameObject nutritionPannelInstance = null;

    [SerializeField] GameObject footprintPannelPrefab;
    [SerializeField] Transform footprintSpawn;

    [SerializeField] Transform container;
    GameObject footprintPannelInstance = null;

    [SerializeField] private UIThemeManager themeManager;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("start Parent");
        themeManager = GetComponentInParent<UIThemeManager>();

        //nur zum test
        SetUpTitlePannel();
        SetUpNutritionPanel();
        SetUpZutatenPanel();
        SetUpFootprintPanel();
        UpdateTheme();
    }

    // Update is called once per frame
    public void SetProductData(Root productRoot)
    {
        Debug.LogError("SetData");
        productData = productRoot;
        SetUpTitlePannel();
        SetUpNutritionPanel();
        UpdateTheme();
    }

    public void SetUpZutatenPanel()
    {
        try
        {
            if (ingredientPannelInstance == null)
            {
                ingredientPannelInstance = Instantiate(ingredientPannelPrefab, ingredientsSpawn.position, ingredientsSpawn.rotation, ingredientsSpawn);
                ingredientPannelInstance.GetComponent<Panel>().SetProductParent(this);
                ingredientPannelInstance.GetComponent<Panel>().SetSpawn(ingredientsSpawn);
                UpdateTheme();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPannel: " + ex.Message);
        }

    }
    public GameObject GetZutantenPanel()
    {
        return ingredientPannelInstance;
    }
    public Transform GetZutatenSpawn()
    {
        return ingredientsSpawn;
    }

    public void SetUpTitlePannel()
    {
        try
        {
            if (titlePanelInstance == null)
            {
                titlePanelInstance = Instantiate(titlePanelPrefab, titleSpawn.position, titleSpawn.rotation, titleSpawn);
                titlePanelInstance.GetComponent<Panel>().SetProductParent(this);
                titlePanelInstance.GetComponent<Panel>().SetSpawn(titleSpawn);
                titlePanelInstance.GetComponent<TitlePanel>().getTitleSection().text = productData.Product.ProductName;
                UpdateTheme();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TitlePannel: " + ex.Message);
        }

    }
    public GameObject GetTitlePanel()
    {
        return titlePanelInstance;
    }
    public Transform GetTitleSpawn()
    {
        return titleSpawn;
    }

    public void SetUpGeminiPannel(string prompt, string response)
    {
        try
        {
            if (geminiPanelInstance == null)
            {
                geminiPanelInstance = Instantiate(gemininPanelPrefab, geminiSpawn.position, geminiSpawn.rotation, geminiSpawn);
                geminiPanelInstance.GetComponent<Panel>().SetProductParent(this);
                geminiPanelInstance.GetComponent<Panel>().SetSpawn(geminiSpawn);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetMenuTitle().text = productData.Product.ProductName;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
                UpdateTheme();
            }
            else
            {
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TitlePannel: " + ex.Message);
        }

    }
    public GameObject GetGeminiPanel()
    {
        return gemininPanelPrefab;
    }
    public Transform GetGeminiSpawn()
    {
        return geminiSpawn;
    }

    public void SetUpNutritionPanel()
    {
        Vector3 offset = new Vector3(0.8f, 0f, 0f);
        try
        {
            if (nutritionPannelInstance == null)
            {
                nutritionPannelInstance = Instantiate(nutritionPannelPrefab, nutritionSpawn.position, nutritionSpawn.rotation, nutritionSpawn);
                nutritionPannelInstance.GetComponent<Panel>().SetProductParent(this);
                nutritionPannelInstance.GetComponent<Panel>().SetSpawn(nutritionSpawn);
                UpdateTheme();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    public GameObject GetNutriPanel()
    {
        return nutritionPannelInstance;
    }
    public Transform GetNutritionSpawn()
    {
        return nutritionSpawn;
    }

    public void SetUpFootprintPanel()
    {
        try
        {
            if (footprintPannelInstance == null)
            {
                footprintPannelInstance = Instantiate(footprintPannelPrefab, footprintSpawn.position, footprintSpawn.rotation, footprintSpawn);
                footprintPannelInstance.GetComponent<Panel>().SetProductParent(this);
                footprintPannelInstance.GetComponent<Panel>().SetSpawn(footprintSpawn);
                UpdateTheme();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    public GameObject GetUmweltPanel()
    {
        return footprintPannelInstance;
    }
    public Transform GetUmweltSpawn()
    {
        return footprintSpawn;
    }

    public Transform GetContainer()
    {
        return container;
    }

    private void UpdateTheme()
    {
        themeManager.ApplyCurrentTheme();
    }
}
