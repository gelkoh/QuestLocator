using System;
using UnityEngine;


public class ProductParent : MonoBehaviour
{
    public Root productData;
    [SerializeField] GameObject BasePannel;
    [SerializeField] GameObject IngredientPannel;
    [SerializeField] GameObject NutritionPannel;
    [SerializeField] GameObject FootprintPannel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("start Parent");
    }

    // Update is called once per frame
    public void SetProductData(Root productRoot)
    {
        Debug.LogError("SetData");
        productData = productRoot;
        SetUpBasePanel();
        SetUpIngredientPanel();
        SetUpNutritionPanel();
        SetUpFootprintPanel();

    }

    private void SetUpBasePanel()
    {
        try
        {
            BasePannel = Instantiate(BasePannel, transform.position, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    private void SetUpIngredientPanel()
    {
        try
        {
            IngredientPannel = Instantiate(IngredientPannel, transform.position, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    private void SetUpNutritionPanel()
    {
        try
        {
            NutritionPannel  = Instantiate(NutritionPannel, transform.position, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }
    private void SetUpFootprintPanel()
    {
        try
        {
            FootprintPannel  = Instantiate(FootprintPannel, transform.position, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPanel: " + ex.Message);
        }
    }

}
