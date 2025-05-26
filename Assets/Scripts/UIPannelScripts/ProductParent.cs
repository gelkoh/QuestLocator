using System;
using UnityEngine;


public class ProductParent : MonoBehaviour
{
    public Root productData;
    [SerializeField] GameObject testPannel;
    
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
        SetUpBasePannel();
    }

    private void SetUpBasePannel()
    {
        try
        {
            testPannel = Instantiate(testPannel, transform.position, transform.rotation, transform);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instancing TestPannel: " + ex.Message);
        }
        
    }
}
