using System;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

    private UIThemeManagerLocal themeManager;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("start Parent");
        //themeManager = GetComponentInParent<UIThemeManagerLocal>();

        //nur zum test
        //SetUpTitlePannel();
        //SetUpNutritionPanel();
        //SetUpZutatenPanel();
        //SetUpFootprintPanel();
        UpdateTheme();
    }

    // Update is called once per frame
    public void SetProductData(Root productRoot)
    {
        Debug.LogError("SetData");
        productData = productRoot;
        themeManager = GetComponentInParent<UIThemeManagerLocal>();
        SetUpTitlePannel();
        SetUpNutritionPanel();
        //UpdateTheme();
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
                //geminiPanelInstance.GetComponent<GeminiPanel>().StopTtsSpeaker();
                geminiPanelInstance.GetComponent<GeminiPanel>().GetMenuTitle().text = productData.Product.ProductName;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
                //geminiPanelInstance.GetComponent<GeminiPanel>().TtsTrigger(response);
                UpdateTheme();
            }
            else
            {
                //geminiPanelInstance.GetComponent<GeminiPanel>().StopTtsSpeaker();
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                //geminiPanelInstance.GetComponent<GeminiPanel>().SetButtons(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
                //geminiPanelInstance.GetComponent<GeminiPanel>().TtsTrigger(response);

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
