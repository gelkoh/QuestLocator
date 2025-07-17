using System;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using TMPro;
using UnityEngine;

public class GeminiPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextSection;
    [SerializeField] TextMeshProUGUI menuTitle;
    [SerializeField] TextMeshProUGUI panelTitle;
    [SerializeField] Transform Buttons;
    [SerializeField] GameObject wordButtonPrefab;
    [Header("TTS")]
    private string TTSText = "Welcome to the tutorial!";

    // Reference to the TTSSpeaker component in this panel
    [SerializeField] private TTSSpeaker ttsSpeaker;
    private String prompt;
    GameObject wissenschaftlichButton;
    String wissenschaftlich = " präzise und wissenschaftlich korrekt in maximal zwei Sätzen. Verwende dabei die Terminologie der jeweiligen Fachdisziplin. Der Text soll ungestyled sein";
    GameObject einfachButton;
    String einfach = " so, dass ihn auch jemand ohne Vorwissen versteht. Die Erklärung soll korrekt, einfach und höchstens zwei kurze Sätze lang sein. Der Text soll ungestyled sein";
    GameObject fuerKinderButton;
    String fuerKinder = "auf einfache, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht. In Maximal 2 Sätzen. Der Text soll ungestyled sein";
    List<GameObject> wordButtonList = new List<GameObject>();
    void Start()
    {
        ProductParent parentPanel = GetComponent<Panel>().GetProductParent();

        wissenschaftlichButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        einfachButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        fuerKinderButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        SetButtons(prompt);
        parentPanel.UpdateTheme();
    }

    public void SetButtons(string prompt)
    {

        wissenschaftlichButton.GetComponentInChildren<TextMeshProUGUI>().text = "Wissenschaftlich";
        wissenschaftlichButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        wissenschaftlichButton.GetComponent<WordButton>().setPromt(prompt);
        wissenschaftlichButton.GetComponent<WordButton>().setPromptSentence(wissenschaftlich);
        wissenschaftlichButton.GetComponent<WordButton>().id = 1;
        wordButtonList.Add(wissenschaftlichButton);
        
        einfachButton.GetComponentInChildren<TextMeshProUGUI>().text = "Einfach";
        einfachButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        einfachButton.GetComponent<WordButton>().setPromt(prompt);
        einfachButton.GetComponent<WordButton>().setPromptSentence(einfach);
        einfachButton.GetComponent<WordButton>().id = 2;
        wordButtonList.Add(einfachButton);

        
        fuerKinderButton.GetComponentInChildren<TextMeshProUGUI>().text = "Für Kinder";
        fuerKinderButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        fuerKinderButton.GetComponent<WordButton>().setPromt(prompt);
        fuerKinderButton.GetComponent<WordButton>().setPromptSentence(fuerKinder);
        fuerKinderButton.GetComponent<WordButton>().id = 3;
        wordButtonList.Add(fuerKinderButton);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI GetTextSection()
    {
        return TextSection;
    }

    public TextMeshProUGUI GetPanelTitle()
    {
        return
        panelTitle;
    }

    public TextMeshProUGUI GetMenuTitle()
    {
        return menuTitle;
    }

    public void SetPrompt(String prompt)
    {
        this.prompt = prompt;
    }

    public List<GameObject> GetWordList()
    {
        return wordButtonList;
    }

    public void TtsTrigger(string text)
    {
        ttsSpeaker = GameObject.FindGameObjectWithTag("TTS").GetComponent<TTSSpeaker>();
        if (ttsSpeaker != null)
        {
            List<String> chunks = SplitIntoChunksWordAware(text, 140);
            foreach (String chunk in chunks)
            {
                ttsSpeaker.SpeakQueued(chunk);
            }
        }
    }

    public void StopTtsSpeaker()
    {
        ttsSpeaker = GameObject.FindGameObjectWithTag("TTS").GetComponent<TTSSpeaker>();
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Stop();
        }
    }

    public static List<string> SplitIntoChunksWordAware(string text, int maxChunkSize)
    {
        List<string> chunks = new List<string>();
        string[] words = text.Split(' ');
        string currentChunk = "";
        
        foreach (string word in words)
        {
            // Check if adding this word would exceed the limit
            string testChunk = string.IsNullOrEmpty(currentChunk) ? word : currentChunk + " " + word;
            
            if (testChunk.Length <= maxChunkSize)
            {
                currentChunk = testChunk;
            }
            else
            {
                // Add current chunk and start a new one
                if (!string.IsNullOrEmpty(currentChunk))
                {
                    chunks.Add(currentChunk);
                }
                currentChunk = word;
                
                // Handle words longer than chunk size
                if (word.Length > maxChunkSize)
                {
                    chunks.Add(word.Substring(0, maxChunkSize));
                    currentChunk = word.Substring(maxChunkSize);
                }
            }
        }
        
        // Add the last chunk if it exists
        if (!string.IsNullOrEmpty(currentChunk))
        {
            chunks.Add(currentChunk);
        }
        
        return chunks;
    }
    
}
