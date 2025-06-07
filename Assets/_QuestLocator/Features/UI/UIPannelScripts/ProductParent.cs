using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProductParent : MonoBehaviour
{
    public Root productData;
    string productName;
    [SerializeField] GameObject testPannelPrefab;
    GameObject testPanelInstance;
    [SerializeField] GameObject titlePanelPrefab;
    GameObject titlePanelInstance;
    [SerializeField] GameObject gemininPanelPrefab;
    GameObject geminiPanelInstance = null;
    [SerializeField] Transform titleSpawn;
    [SerializeField] Transform umweltauswirkungenSpawn;
    [SerializeField] Transform zutatenSpawn;
    [SerializeField] Transform geminiSpawn;


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
    }

    private void SetUpZutatenPanel()
    {
        try
        {
            testPanelInstance = Instantiate(testPannelPrefab, zutatenSpawn.position, zutatenSpawn.rotation, zutatenSpawn);
            testPanelInstance.GetComponent<Panel>().SetProductParent(this);
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

    public void RemovePanel(GameObject panel)
    {
        //remove the instance so a new can be spawend
    }
}
