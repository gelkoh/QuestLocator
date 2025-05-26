using System;
using UnityEngine;
using TMPro;

public class ProductScript : MonoBehaviour
{
    public Root productData;
    //public GameObject ingredientUI;
    private GameObject myItem;
    private GameObject basePannel;

    void Start()
    {
        //myItem = Instantiate(Resources.Load("Cube")) as GameObject;

        //Debug.Log(myItem);
    }
    public void createUI()
    {

        name = productData.Product.ProductName;
        Debug.Log("end");
    }
    public void setProductData(Root productRoot)
    {
        productData = productRoot;
        setUpBasePannel();
    }
    private void setUpBasePannel()
    {
        Debug.LogError("setupbase------");
        try
        {
            myItem = Resources.Load("UiPannels/TestIngredientsPannel") as GameObject;
            basePannel = Instantiate(myItem, transform.position, transform.rotation, transform);
            Debug.LogError(basePannel.transform.GetComponentsInChildren<Transform>(true));
            Debug.Log("child count: " + transform.childCount);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception in createProduct: " + ex.Message);
        }
        try
        {
            var textSection = basePannel.transform.Find("CanvasRoot/UIBackplate/InfoSect/ingredients");
            Debug.Log(textSection);
            
            foreach (var item in productData.Product.Ingredients)
            {
                GameObject textObj = new GameObject("UIText", typeof(TextMeshProUGUI)); //create new tmp object

                textObj.transform.SetParent(textSection.transform, false);  //set it to be child of text section 

                TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>(); //get tmpGui section
                //style
                tmp.text = item.Text;
                tmp.fontSize = 20;
                //tmp.alignment = TextAlignmentOptions.Center;
                //Stmp.color = Color.green;
            }
            
            

        }
        catch (Exception ex)
        {
            Debug.LogError("Error in setting Text: " + ex.Message);
        }
        
    }
}
