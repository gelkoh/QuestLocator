using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class MyBehaviour : MonoBehaviour
{
    string url = "https://world.openfoodfacts.net/api/v2/product/3017624010701";
    
    
    void Start()
    {
        //StartCoroutine("GetText");
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
                Debug.Log(myDeserializedClass.product._keywords);
            }
        }
    }

    void Update()
    {

    }
    
}
