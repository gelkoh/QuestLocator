using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProductParent : MonoBehaviour
{
    public Root productData;
<<<<<<< HEAD
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

=======
    [SerializeField] GameObject BasePannel;
    [SerializeField] GameObject IngredientPannel;
    [SerializeField] GameObject NutritionPannel;
    [SerializeField] GameObject FootprintPannel;
>>>>>>> e1ea929089e0ec48db3790a893c47a79078a625d

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
<<<<<<< HEAD
        SetUpZutatenPanel();
=======
        SetUpBasePanel();
        SetUpIngredientPanel();
        SetUpNutritionPanel();
        SetUpFootprintPanel();

>>>>>>> e1ea929089e0ec48db3790a893c47a79078a625d
    }

    private void SetUpZutatenPanel()
    {
        try
        {
<<<<<<< HEAD
            testPanelInstance = Instantiate(testPannelPrefab, zutatenSpawn.position, zutatenSpawn.rotation, zutatenSpawn);
            testPanelInstance.GetComponent<Panel>().SetProductParent(this);
=======
            BasePannel = Instantiate(BasePannel, transform.position, transform.rotation, transform);
>>>>>>> e1ea929089e0ec48db3790a893c47a79078a625d
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPannel: " + ex.Message);
        }
<<<<<<< HEAD

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
=======
>>>>>>> e1ea929089e0ec48db3790a893c47a79078a625d
    }
    private void SetUpIngredientPanel()
    {
        Vector3 offset = new Vector3(0.4f, 0f, 0f);
        try
        {
            IngredientPannel = Instantiate(IngredientPannel, transform.position+offset, transform.rotation, transform);
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
            NutritionPannel = Instantiate(NutritionPannel, transform.position+offset, transform.rotation, transform);
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
            FootprintPannel = Instantiate(FootprintPannel, transform.position+ offset, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }

}
