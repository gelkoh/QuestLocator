using UnityEngine;

public class rando : MonoBehaviour
{
    private BarcodeProcessor _barcodeProcessor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _barcodeProcessor = new BarcodeProcessor();
        Debug.Log("START-------------------");
        _barcodeProcessor.GetProductData("3017624010701");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
