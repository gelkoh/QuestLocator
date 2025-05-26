using System;
using UnityEngine;
using TMPro;

public class DebugPannelScript : MonoBehaviour
{
    
    private BarcodeProcessor _barcodeProssor;
    private GameObject tempPrefab;
    private GameObject basePannel;


    private String barcode = "4001475104602";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _barcodeProssor = new BarcodeProcessor();
    }
    public void OnClick()
    {
        outputError("Debug code run");
        createProduct();
    }
    private void outputError(String output)
    {
        Debug.LogError(output);
    }
    private void createProduct()
    {

        try
        {
            tempPrefab = Resources.Load("UiPannels/TestIngredientsPannel") as GameObject;
            basePannel = Instantiate(tempPrefab); //create new pannel
            basePannel.transform.position = new Vector3(0, 2f, 0.3f);

        }
        catch (Exception ex)
        {
            Debug.LogError("Exception in createProduct: " + ex.Message);
        }
        
        try
        {
            var textSection = basePannel.transform.Find("CanvasRoot/UIBackplate/InfoSect/ingredients"); //load section where text should go

            GameObject textObj = new GameObject("UIText", typeof(TextMeshProUGUI)); //create new tmp object
            
            textObj.transform.SetParent(textSection.transform, false);  //set it to be child of text section 

            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>(); //get tmpGui section
            //style
            tmp.text = "Hello UI!";
            tmp.fontSize = 36;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.green;

        }
        catch (Exception ex)
        {
            Debug.LogError("Exception in createProduct: " + ex.Message);
        }

        
    }
}
