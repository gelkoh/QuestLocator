using System.Collections.Generic;
using Meta.WitAi;
using TMPro;
using UnityEngine;

public class WordButton : MonoBehaviour
{
    APIManager aPI_Manager;
    Panel parentPanel;
    [SerializeField] GameObject loadingPrefab;
    [SerializeField] GameObject ActiveIndicator;
    [SerializeField] GameObject Gap;
    public int id { get; set; } 
    string promptWord;
    string promptSentence;
    List<GameObject> list;
    void Start()
    {
        aPI_Manager = GameObject.FindWithTag("API_Manager").GetComponent<APIManager>();
        if (parentPanel.gameObject.GetComponent<IngredientPannel>() != null)
        {
            list = parentPanel.gameObject.GetComponent<IngredientPannel>().GetWordList();
        }else if(parentPanel.gameObject.GetComponent<GeminiPanel>() != null)
        {
            list = parentPanel.gameObject.GetComponent<GeminiPanel>().GetWordList();
        }
    }

    public void SendPrompt()
    {
        foreach (var button in list)
        {
            if (button.GetComponent<WordButton>().id == id)
            {
                button.GetComponent<WordButton>().setActiveIndicator(true);
            }
            else
            {
                button.GetComponent<WordButton>().setActiveIndicator(false);
            }
        }

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

    public void setActiveIndicator(bool activeIndicator)
    {
        Debug.LogError(activeIndicator + " id:"+id+" promt:" + promptWord);
        Gap.SetActive(activeIndicator);
        ActiveIndicator.SetActive(activeIndicator);
    }
}
