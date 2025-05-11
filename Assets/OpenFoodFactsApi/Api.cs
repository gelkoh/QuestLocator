using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.InputSystem;

public class OpenFoodFactsAPI : MonoBehaviour
{
    string baseUrl = "https://world.openfoodfacts.net/api/v2/product/";
    //Root[] scannedProducts = ArrayList<Root>();

    public IEnumerator MakeAPICall(string ean)
    {
        string requestUrl = baseUrl + ean;

        Debug.Log("in");
        using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
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
}