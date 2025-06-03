using TMPro;
using UnityEngine;

public class WordButton : MonoBehaviour
{
    APIManager aPI_Manager;
    [SerializeField] GameObject aiHelperPrefab;
    [SerializeField] TextMeshProUGUI prompt;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aPI_Manager = GameObject.FindWithTag("API_Manager").GetComponent<APIManager>();
    }

    public void SendPrompt()
    {
        aPI_Manager.GetAiResponse(prompt.text,(response) =>
        {
            Transform contentRoot = GameObject.FindWithTag("ContentRoot").GetComponent<Transform>();
            GameObject responseDisplayInstance = Instantiate(aiHelperPrefab, contentRoot);
            responseDisplayInstance.GetComponent<AIHelperPanel>().GetTextSection().text = response;
            responseDisplayInstance.GetComponent<AIHelperPanel>().GetTitle().text = prompt.text + " Explained";
        });
       
    }
}
