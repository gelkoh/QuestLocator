using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class API_Manager : MonoBehaviour
{
    [SerializeField] private string gasURL; //Google App Script URL

     private string response = "";
    
    
    public string getAiResponse(string promptWord)
    {
        string prompt = "Erkläre das Wort "  + promptWord + "auf einfache, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht.";
        StartCoroutine(SendDataToGAS(prompt));
        return response;
    }

    private IEnumerator SendDataToGAS(string prompt){ //sends prompt to gasURL via form.parameter and recieves response from AI
        WWWForm form = new WWWForm();
        form.AddField("parameter",prompt);
        UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        yield return www.SendWebRequest();
        

        if(www.result == UnityWebRequest.Result.Success){
            response = www.downloadHandler.text;
        }
        else{
            response = "There was an error.";
        }
        
    }
}
