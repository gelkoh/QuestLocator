using System;
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






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("start Parent");

        //nur zum test
        SetUpTitlePannel();
        SetUpNutritionPanel();
        SetUpZutatenPanel();
        SetUpFootprintPanel();
    }

    // Update is called once per frame
    public void SetProductData(Root productRoot)
    {
        Debug.LogError("SetData");
        productData = productRoot;
        SetUpTitlePannel();
        SetUpNutritionPanel();
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
                Debug.Log("setSpawn" + geminiSpawn);
                geminiPanelInstance.GetComponent<Panel>().SetSpawn(geminiSpawn);
                geminiPanelInstance.GetComponent<GeminiPanel>().StopTtsSpeaker();
                geminiPanelInstance.GetComponent<GeminiPanel>().GetMenuTitle().text = productData.Product.ProductName;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
                geminiPanelInstance.GetComponent<GeminiPanel>().TtsTrigger(response);
            }
            else
            {
                geminiPanelInstance.GetComponent<GeminiPanel>().StopTtsSpeaker();
                geminiPanelInstance.GetComponent<GeminiPanel>().SetPrompt(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().SetButtons(prompt);
                geminiPanelInstance.GetComponent<GeminiPanel>().GetTextSection().text = response;
                geminiPanelInstance.GetComponent<GeminiPanel>().GetPanelTitle().text = prompt + " Explained";
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

}
