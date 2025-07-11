using System;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProductParent : MonoBehaviour
{
    public Root productData;

    [SerializeField] GameObject titlePanelPrefab;
    [SerializeField] Transform titleSpawn;
    [SerializeField] GameObject titlePanelInstance;

    [SerializeField] GameObject gemininPanelPrefab;
    [SerializeField] Transform geminiSpawn;
    [SerializeField] GameObject geminiPanelInstance = null;

    [SerializeField] GameObject ingredientPannelPrefab;
    [SerializeField] Transform ingredientsSpawn;
    [SerializeField] GameObject ingredientPannelInstance = null;

    [SerializeField] GameObject nutritionPannelPrefab;
    [SerializeField] Transform nutritionSpawn;
    [SerializeField] GameObject nutritionPannelInstance = null;

    [SerializeField] GameObject footprintPannelPrefab;
    [SerializeField] Transform footprintSpawn;

    [SerializeField] Transform container;
    [SerializeField] GameObject footprintPannelInstance = null;
    [SerializeField] GameObject panelMover;


    private Color _productBorderColorA;
    private Color _productBorderColorB;

    private List<Color> _availableBorderColorsA = new() { Color.blue, Color.green, Color.yellow, Color.magenta, new Color(255, 255, 0), Color.cyan };
    private List<Color> _availableBorderColorsB = new() { Color.blue * 0.5f, Color.green * 0.5f, Color.yellow * 0.5f, Color.magenta * 0.5f, new Color(255, 255, 0) * 0.5f, Color.cyan * 0.5f }; 

    private static int _nextColorIndex = 0;

    private UIThemeManagerLocal themeManager;

    void Start()
    {
        themeManager = GetComponentInParent<UIThemeManagerLocal>();

        if (themeManager == null)
        {
            Debug.Log("[ProductParent]: Theme manager local couldn't be found in parent.");
        }
        else
        {
            themeManager.OnThemeApplied += ApplyBorderColorToAllPanels;      
        }

        UpdateTheme();
        Debug.Log("ProductParent Start method called.");
    }
    
    void OnDestroy()
    {
        themeManager.OnThemeApplied -= ApplyBorderColorToAllPanels;  
    }

    public void SetProductData(Root productRoot)
    {
        Debug.Log("[ProductParent] SetProductData called for product: " + productRoot.Product.ProductName);
        productData = productRoot;
        themeManager = GetComponentInParent<UIThemeManagerLocal>();

        SetProductGroupBorderColors();

        SetUpTitlePannel();
        SetUpNutritionPanel();

        UpdateTheme();
        ApplyBorderColorToAllPanels();
    }

    private void SetProductGroupBorderColors()
    {
        _productBorderColorA = _availableBorderColorsA[_nextColorIndex % _availableBorderColorsA.Count];
        _productBorderColorB = _availableBorderColorsB[_nextColorIndex % _availableBorderColorsB.Count];

        Debug.Log("[ProductParent nextColorInndex]: " + _nextColorIndex);
        _nextColorIndex++;
        Debug.Log($"[ProductParent] Assigned Border Colors for this product group: A={_productBorderColorA}, B={_productBorderColorB}");
    }

    private void ApplyBorderColorToPanel(GameObject panelInstance)
    {
        if (panelInstance == null)
        {
            Debug.LogWarning($"[ProductParent] Panel instance is null. Cannot apply border color.");
            return;
        }

        Transform canvasRootTransform = panelInstance.transform.Find("CanvasRoot");

        if (canvasRootTransform == null)
        {
            Debug.LogWarning($"[ProductParent] No 'CanvasRoot' child found on {panelInstance.name}. Cannot apply border color.");
            return;
        }

        Transform uiBackplateTransform = canvasRootTransform.Find("UIBackplate");

        if (uiBackplateTransform == null)
        {
            Debug.LogWarning($"[ProductParent] No 'UIBackplate' child found under 'CanvasRoot' in {panelInstance.name}. Cannot apply border color.");
            return;
        }

        Image panelImage = uiBackplateTransform.GetComponent<Image>();
        if (panelImage == null)
        {
            Debug.LogWarning($"[ProductParent] No Image component found on 'UIBackplate' of {panelInstance.name}. Cannot apply border color.");
            return;
        }

        Material materialInstance = panelImage.material;
        if (materialInstance == null)
        {
            Debug.LogWarning($"[ProductParent] No material found on Image component of 'UIBackplate' of {panelInstance.name}. Cannot apply border color.");
            return;
        }

        materialInstance.SetColor("_BorderColorA", _productBorderColorA);
        materialInstance.SetColor("_BorderColorB", _productBorderColorB);

        Debug.Log($"[ProductParent] Applied border colors to {panelInstance.name}: A={_productBorderColorA}, B={_productBorderColorB}");
    }

    private void ApplyBorderColorToPanelMover(GameObject panelInstance)
    {
        if (panelInstance == null)
        {
            Debug.LogWarning($"[ProductParent PanelMover] Panel instance is null. Cannot apply border color.");
            return;
        }

        // Transform canvasRootTransform = panelInstance.transform.Find("CanvasRoot");

        // if (canvasRootTransform == null)
        // {
        //     Debug.LogWarning($"[ProductParent PanelMover] No 'CanvasRoot' child found on {panelInstance.name}. Cannot apply border color.");
        //     return;
        // }

        Transform uiBackplateTransform = panelInstance.transform.Find("UIBackplate");

        if (uiBackplateTransform == null)
        {
            Debug.LogWarning($"[ProductParent PanelMover] No 'UIBackplate' child found under 'CanvasRoot' in {panelInstance.name}. Cannot apply border color.");
            return;
        }

        Image panelImage = uiBackplateTransform.GetComponent<Image>();

        if (panelImage == null)
        {
            Debug.LogWarning($"[ProductParent PanelMover] No Image component found on 'UIBackplate' of {panelInstance.name}. Cannot apply border color.");
            return;
        }

        Material materialInstance = panelImage.material;

        if (materialInstance == null)
        {
            Debug.LogWarning($"[ProductParent PanelMover] No material found on Image component of 'UIBackplate' of {panelInstance.name}. Cannot apply border color.");
            return;
        }

        materialInstance.SetColor("_BorderColorA", _productBorderColorA);
        materialInstance.SetColor("_BorderColorB", _productBorderColorB);

        Debug.Log($"[ProductParent PanelMover] Applied border colors to {panelInstance.name}: A={_productBorderColorA}, B={_productBorderColorB}");
    }

    private void ApplyBorderColorToAllPanels()
    {
        Debug.Log("[ProductParent] Applying border color to all panels.");
        ApplyBorderColorToPanel(titlePanelInstance);
        ApplyBorderColorToPanel(geminiPanelInstance);
        ApplyBorderColorToPanel(ingredientPannelInstance);
        ApplyBorderColorToPanel(nutritionPannelInstance);
        ApplyBorderColorToPanel(footprintPannelInstance);
        ApplyBorderColorToPanelMover(panelMover);
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
                ingredientPannelInstance.GetComponent<IngredientPannel>().FillInfo();
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
                Debug.LogError("start gemini pannel");
                geminiPanelInstance = Instantiate(gemininPanelPrefab, geminiSpawn.position, geminiSpawn.rotation, geminiSpawn);
                geminiPanelInstance.GetComponent<Panel>().SetProductParent(this);
                geminiPanelInstance.GetComponent<Panel>().SetSpawn(geminiSpawn);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetMenuTitle().text = productData.Product.ProductName;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
                //geminiPanelInstance.GetComponent<GeminiPanel>().SetButtons(prompt);
                Debug.LogError("update Theme gemini");
                UpdateTheme();
                geminiPanelInstance.GetComponent<GeminiPanel>().StopTtsSpeaker();
                geminiPanelInstance.GetComponent<GeminiPanel>().TtsTrigger(response);

            }
            else
            {
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().SetButtons(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
                geminiPanelInstance.GetComponent<GeminiPanel>().StopTtsSpeaker();
                geminiPanelInstance.GetComponent<GeminiPanel>().TtsTrigger(response);

            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing GeminiPannel: " + ex.Message);
        }

    }

    public GameObject GetGeminiPanel()
    {
        return geminiPanelInstance;
    }

    public Transform GetGeminiSpawn()
    {
        return geminiSpawn;
    }

    public void SetUpNutritionPanel()
    {
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
                footprintPannelInstance.GetComponent<Footprint>().FillInfo();
                Debug.LogError("update footprintTheme");
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
        if (themeManager != null)
        {
            themeManager.ApplyCurrentTheme();
        }
        else
        {
            Debug.LogWarning("[ProductParent] UIThemeManagerLocal not found or assigned. Theme update skipped.");
        }
    }
}
