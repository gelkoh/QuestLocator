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
    String wissenschaftlich = " präzise und wissenschaftlich korrekt in maximal zwei Sätzen. Verwende dabei die Terminologie der jeweiligen Fachdisziplin. Der Text soll ungestyled sein";
    String einfach = " so, dass ihn auch jemand ohne Vorwissen versteht. Die Erklärung soll korrekt, einfach und höchstens zwei kurze Sätze lang sein. Der Text soll ungestyled sein";
    String fuerKinder = "auf einfache, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht. In Maximal 2 Sätzen. Der Text soll ungestyled sein";

    void Start()
    {
        GameObject wissenschaftlichButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        wissenschaftlichButton.GetComponentInChildren<TextMeshProUGUI>().text = "Wissenschaftlich";
        wissenschaftlichButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        wissenschaftlichButton.GetComponent<WordButton>().setPromt(prompt);
        wissenschaftlichButton.GetComponent<WordButton>().setPromptSentence(wissenschaftlich);

        GameObject einfachButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        einfachButton.GetComponentInChildren<TextMeshProUGUI>().text = "Einfach";
        einfachButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        einfachButton.GetComponent<WordButton>().setPromt(prompt);
        einfachButton.GetComponent<WordButton>().setPromptSentence(einfach);


        GameObject fuerKinderButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        fuerKinderButton.GetComponentInChildren<TextMeshProUGUI>().text = "Für Kinder";
        fuerKinderButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        fuerKinderButton.GetComponent<WordButton>().setPromt(prompt);
        fuerKinderButton.GetComponent<WordButton>().setPromptSentence(fuerKinder);

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

    public void TtsTrigger(string text)
    {
        ttsSpeaker = GameObject.FindGameObjectWithTag("TTS").GetComponent<TTSSpeaker>();
        if (ttsSpeaker != null)
        {
            if (text.Length > 140)
            {
                List<String> chunks = SplitIntoChunksWordAware(text, 140);
                foreach (String chunk in chunks)
                {
                    ttsSpeaker.SpeakQueued(chunk);
                }
            }
        }
        else
        {
            Debug.LogWarning("TTSSpeaker not found on IntroPanel. Please add a TTSSpeaker component as a child.");
        }
    }

    public void StopTtsSpeaker()
    {
        ttsSpeaker = GameObject.FindGameObjectWithTag("TTS").GetComponent<TTSSpeaker>();
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Stop();
        }
        else
        {
            Debug.LogWarning("TTSSpeaker not found on IntroPanel. Please add a TTSSpeaker component as a child.");
        }
    }

    private List<string> SplitIntoChunks(string text, int chunkSize)
    {
        List<string> chunks = new List<string>();

        for (int i = 0; i < text.Length; i += chunkSize)
        {
            int length = Mathf.Min(chunkSize, text.Length - i);
            chunks.Add(text.Substring(i, length));
        }

        return chunks;
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
