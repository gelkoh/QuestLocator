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
    private string baseUrl = "https://world.openfoodfacts.net/api/v2/product/";
    private string endUrlTags = "?fields=_id,product_name,brands_tags,product_quantity,product_quantity_unit,ingredients,ingredients_analysis_tags,ecoscore_grade,nutriscore_grade";
    

    
    public IEnumerator GetProductByEan(string ean, Action<Root> onSuccess, Action<string> onError = null)
    {
        string requestUrl = $"{baseUrl}{ean}{endUrlTags}";
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
                    Root apiProductData = JsonConvert.DeserializeObject<Root>(json);
                    //Debug.Log(productData.product.product_name_en);
                    
                    //Debug.Log(templateUI);
                    /* 
                    GameObject instance = Instantiate(productManagerObj);
                    
                    ProductManager productScript = instance.GetComponent<ProductManager>();
                    productScript.productData = apiProductData;
                    
                    Debug.Log(productScript.productData.Product.ProductName);
                    productScript.createUI(); 
                    */
                    onSuccess?.Invoke(apiProductData);
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