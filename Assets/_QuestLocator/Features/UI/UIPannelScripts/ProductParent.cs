using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProductParent : MonoBehaviour
{
    public Root productData;
    string productName;
    [SerializeField] GameObject basePannelPrefab;
    GameObject basePannelInstance;

    [SerializeField] GameObject titlePanelPrefab;
    [SerializeField] Transform titleSpawn;
    GameObject titlePanelInstance;

    [SerializeField] GameObject gemininPanelPrefab;
    [SerializeField] Transform geminiSpawn;
    GameObject geminiPanelInstance = null;

    [SerializeField] GameObject IngredientPannelPrefab;
    [SerializeField] Transform ingredientsSpawn;
    GameObject IngredientPannelInstance = null;

    [SerializeField] GameObject NutritionPannel;
    [SerializeField] Transform NutritionSpawn;
    GameObject NutritionPannelInstance = null;

    [SerializeField] GameObject FootprintPannelPrefab;
    [SerializeField] Transform FootprintSpawn;
    GameObject FootprintPannelInstance = null;

    
    
    
    

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
        SetUpIngredientPanel();
        SetUpNutritionPanel();
        SetUpFootprintPanel();

    }

    private void SetUpZutatenPanel()
    {
        try
        {
            basePannelInstance = Instantiate(basePannelPrefab, ingredientsSpawn.position, ingredientsSpawn.rotation, ingredientsSpawn);
            basePannelInstance.GetComponent<Panel>().SetProductParent(this);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPannel: " + ex.Message);
        }

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

    private void SetUpIngredientPanel()
    {
        Vector3 offset = new Vector3(0.4f, 0f, 0f);
        try
        {
            IngredientPannelPrefab = Instantiate(IngredientPannelPrefab, transform.position+offset, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    
    private void SetUpNutritionPanel()
    {
        Vector3 offset = new Vector3(0.8f, 0f, 0f);
        try
        {
            NutritionPannel = Instantiate(NutritionPannel, transform.position + offset, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    
    private void SetUpFootprintPanel()
    {
        Vector3 offset = new Vector3(1.2f, 0f, 0f);
        try
        {
            FootprintPannelPrefab = Instantiate(FootprintPannelPrefab, transform.position + offset, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }

}
