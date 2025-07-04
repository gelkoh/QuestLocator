using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class WordList : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> contentSections;
    [SerializeField] GameObject wordButtonPrefab;
    [SerializeField] Transform contentTransform;
    [SerializeField] Panel parentPanel;
    List<GameObject> wordButtonList = new List<GameObject>();
    

    // TODO: Make this globally accessible since it's also used in GeminiPanel.cs
    String wissenschaftlich = " präzise und wissenschaftlich korrekt in maximal zwei Sätzen. Verwende dabei die Terminologie der jeweiligen Fachdisziplin. Der Text soll ungestyled sein";
    
    String einfach = " so, dass ihn auch jemand ohne Vorwissen versteht. Die Erklärung soll korrekt, einfach und höchstens zwei kurze Sätze lang sein. Der Text soll ungestyled sein";

    String fuerKinder = " auf einfache, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht. In Maximal 2 Sätzen. Der Text soll ungestyled sein";

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        for (int i = 0; i < contentSections.Count; i++)
        {
            string text = contentSections[i].text;
        
            char[] punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();
            char[] whiteSpace = text.Where(Char.IsWhiteSpace).Distinct().ToArray();
            IEnumerable<string> words = text.Split().Select(x => x.Trim(punctuation));
            
            foreach (string word in words)
            {
                if (word.Length > 0)
                {
                    GameObject wordButtonInstance = Instantiate(wordButtonPrefab, contentTransform);
                    wordButtonInstance.GetComponentInChildren<TextMeshProUGUI>().text = word;
                    wordButtonInstance.GetComponent<WordButton>().SetParentPanel(parentPanel);
                    wordButtonInstance.GetComponent<WordButton>().setPromt(word);
                    // wordButtonInstance.GetComponent<WordButton>().setPromptSentence( " auf einfache, kurze aber präzise Weise, sodass jeder die grundlegende Funktion oder Bedeutung versteht.");

                    Debug.Log("WordList: Getting last set translation style index: " + PlayerPrefs.GetInt("TranslationStyleIndex") + " and setting translation style for each button.");

                    if (PlayerPrefs.GetInt("TranslationStyleIndex") == 0)
                    {
                        wordButtonInstance.GetComponent<WordButton>().setPromptSentence(einfach);
                    }
                    if (PlayerPrefs.GetInt("TranslationStyleIndex") == 1)
                    {
                        wordButtonInstance.GetComponent<WordButton>().setPromptSentence(wissenschaftlich);
                    }
                    else if (PlayerPrefs.GetInt("TranslationStyleIndex") == 2)
                    {
                        wordButtonInstance.GetComponent<WordButton>().setPromptSentence(einfach);
                    }
                    else if (PlayerPrefs.GetInt("TranslationStyleIndex") == 3)
                    {
                        wordButtonInstance.GetComponent<WordButton>().setPromptSentence(fuerKinder);
                    }

                    wordButtonList.Add(wordButtonInstance);
                }
            }
        }
    }
}