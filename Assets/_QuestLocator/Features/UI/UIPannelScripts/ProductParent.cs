using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProductParent : MonoBehaviour
{
    public Root productData;
    string productName;

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
    GameObject footprintPannelInstance = null;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("start Parent");

        //nur zum test
        SetUpTitlePannel();
        SetUpZutatenPanel();
    }

    // Update is called once per frame
    public void SetProductData(Root productRoot)
    {
        Debug.LogError("SetData");
        productData = productRoot;
        SetUpZutatenPanel();
        SetUpNutritionPanel();
        SetUpFootprintPanel();

    }

    private void SetUpZutatenPanel()
    {
        try
        {
            ingredientPannelInstance = Instantiate(ingredientPannelPrefab, ingredientsSpawn.position, ingredientsSpawn.rotation, ingredientsSpawn);
            ingredientPannelInstance.GetComponent<Panel>().SetProductParent(this);
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
    private void SetUpTitlePannel()
    {
        try
        {
            titlePanelInstance = Instantiate(titlePanelPrefab, titleSpawn.position, titleSpawn.rotation, titleSpawn);
            titlePanelInstance.GetComponent<Panel>().SetProductParent(this);
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

    public void SetUpGeminiPannel(string prompt, string response)
    {
        try
        {
            if (geminiPanelInstance == null)
            {
                geminiPanelInstance = Instantiate(gemininPanelPrefab, geminiSpawn.position, geminiSpawn.rotation, geminiSpawn);
                geminiPanelInstance.GetComponent<Panel>().SetProductParent(this);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetMenuTitle().text = productName;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
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
    private void SetUpNutritionPanel()
    {
        Vector3 offset = new Vector3(0.8f, 0f, 0f);
        try
        {
            nutritionPannelInstance = Instantiate(nutritionPannelPrefab, nutritionSpawn.position, nutritionSpawn.rotation, nutritionSpawn);
            nutritionPannelInstance.GetComponent<Panel>().SetProductParent(this);
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

    private void SetUpFootprintPanel()
    {
        try
        {
            footprintPannelPrefab = Instantiate(footprintPannelPrefab, footprintSpawn.position, footprintSpawn.rotation, footprintSpawn);
            footprintPannelPrefab.GetComponent<Panel>().SetProductParent(this);
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

}
