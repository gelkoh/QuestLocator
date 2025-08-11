using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System;
using ZXing;

public class OpenFoodFactsClient
{
    private string baseUrl = "https://world.openfoodfacts.net/api/v2/product/";
    private string endUrlTags = "?fields=_id,product_name,brands_tags,product_quantity,product_quantity_unit,ingredients,ingredients_analysis_tags,allergens_tags,nutriments,ecoscore_grade,nutriscore_grade,ecoscore_data,ecoscore_grade,ecoscore_score";

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
                Debug.Log("RAW JSON:\n" + json);

                Root apiProductData = null;
                try
                {
                    apiProductData = JsonConvert.DeserializeObject<Root>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Deserialization error: {ex.Message}");
                    onError?.Invoke(ex.Message);
                    yield break;
                }

                if (apiProductData?.Product == null)
                {
                    Debug.LogError("Produktdaten fehlen oder Produkt ist null.");
                    onError?.Invoke("Produktdaten fehlen oder unvollständig.");
                    yield break;
                }

                onSuccess?.Invoke(apiProductData);
            }
        }
    }

    internal object GetProductByEan(Result ean, Action<Root> onSuccess, Action<string> onError)
    {
        throw new NotImplementedException();
    }
}