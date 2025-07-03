using TMPro;
using UnityEngine;

public class WordButton : MonoBehaviour
{
    APIManager aPI_Manager;
    Panel parentPanel;
    string promptWord;
    string promptSentence;
    void Start()
    {
        aPI_Manager = GameObject.FindWithTag("API_Manager").GetComponent<APIManager>();
    }

    public void SendPrompt()
    {
        aPI_Manager.GetAiResponse(promptWord, promptSentence,(response) =>
        {
            parentPanel.GetProductParent().SetUpGeminiPannel(promptWord, response);
        });
    }

    public void SetParentPanel(Panel parentPanel)
    {
        this.parentPanel = parentPanel;
    }

    public void setPromt(string promptWord)
    {
        this.promptWord = promptWord;
    }

    public void setPromptSentence(string promptSentence)
    {
        this.promptSentence = promptSentence;
    }
}
