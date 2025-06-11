using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    [SerializeField] private string gasURL; //Google App Script URL

     private string response = "";

    public void GetAiResponse(string promptWord, string promptSentence, System.Action<string> callback)
    {
        string prompt = "Erkläre das Wort " + promptWord + promptSentence;
        StartCoroutine(SendDataToGAS(prompt, callback));
    }
    private IEnumerator SendDataToGAS(string prompt, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);
        UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
        }
        else
        {
            response = "There was an error.";
        }

        callback?.Invoke(response);
    }
}
