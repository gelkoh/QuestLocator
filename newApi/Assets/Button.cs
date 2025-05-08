using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ZXing;

public class Button : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI uiText;
    private TMP_InputField tmpInputField;
    
    private string url;
    

    public void OnClick()
    {
        tmpInputField = GetComponentInChildren<TMP_InputField>();
        Debug.Log(tmpInputField.text);
        if (tmpInputField != null)
        {
            
            url = "https://world.openfoodfacts.net/api/v2/product/" + tmpInputField.text;
            Debug.Log(url);
            StartCoroutine("GetText");
        }else
        {
            uiText.text = "Please enter a valid barcode";
        }
        
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GetText()
    {       
        Debug.Log("in");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error:" + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log(json);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                uiText.text = 
                myDeserializedClass.product._id + "\n" +
                myDeserializedClass.product.product_name + "\n";
                for (int i = 0; i < myDeserializedClass.product.ingredients.Count; i++)
                {
                    uiText.text += myDeserializedClass.product.ingredients[i].text + "\n";
                }
                
            }
        }
    }
}
