using Meta.WitAi;
using TMPro;
using UnityEngine;

public class WordButton : MonoBehaviour
{
    APIManager aPI_Manager;
    [SerializeField]Panel parentPanel;
    [SerializeField] GameObject loadingPrefab;
    string promptWord;
    string promptSentence;
    void Start()
    {
        aPI_Manager = GameObject.FindWithTag("API_Manager").GetComponent<APIManager>();
    }

    public void SendPrompt()
    {
        Vector3 pos = new Vector3(parentPanel.GetProductParent().GetGeminiSpawn().position.x, parentPanel.GetProductParent().GetGeminiSpawn().position.y, (float)(parentPanel.GetProductParent().GetGeminiSpawn().position.z - 0.0011));
        GameObject loadingInstance = Instantiate(loadingPrefab, pos, parentPanel.GetProductParent().GetGeminiSpawn().rotation, parentPanel.GetProductParent().GetGeminiSpawn());
        aPI_Manager.GetAiResponse(promptWord, promptSentence, (response) =>
        {
            loadingInstance.DestroySafely();
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
