using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using System;
using ZXing;

public class OpenFoodFactsClient
{
    // string baseUrl = "https://world.openfoodfacts.net/api/v2/product/";
    //Root[] scannedProducts = ArrayList<Root>();

    // public IEnumerator GetProductByEan(string ean)
    // {
    //     string requestUrl = baseUrl + ean;

    //     Debug.Log("in");
    //     using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
    //     {
    //         yield return request.SendWebRequest();
    //         if (request.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.Log("Error:" + request.error);
    //         }
    //         else
    //         {
    //             string json = request.downloadHandler.text;
    //             Debug.Log(json);
    //             Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
    //             Debug.Log(myDeserializedClass.product._keywords);
    //         }
    //     }
    // }

    private string baseUrl = "https://world.openfoodfacts.net/api/v2/product/";

    public IEnumerator GetProductByEan(string ean, Action<Root> onSuccess, Action<string> onError = null)
    {
        string requestUrl = $"{baseUrl}{ean}";
        using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error: {request.error}");
                onError?.Invoke(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                try
                {
                    Root productData = JsonConvert.DeserializeObject<Root>(json);
                    onSuccess?.Invoke(productData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Deserialization error: {ex.Message}");
                    onError?.Invoke(ex.Message);
                }
            }
        }
    }

    internal object GetProductByEan(Result ean, Action<Root> onSuccess, Action<string> onError)
    {
        throw new NotImplementedException();
    }
}