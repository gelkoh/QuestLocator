using System;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
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
    private TTSSpeaker ttsSpeaker;
    private String prompt;
    GameObject ScientificButton;
    String Scientific = " precisely and scientifically correct with a maximum of 434 characters. Use the terminology of the respective discipline. The text should be unstyled";
    GameObject NormalButton;
    String Normal = " in such a way that someone without prior knowledge can understand it. The explanation should be correct, normal and no longer than 434 characters. The text should be unstyled";
    GameObject forChildrenButton;
    String forChildren = " in a normal, short but precise way so that a child understands the basic function or meaning. In a maximum of 434 characters. The text should be unstyled";
    List<GameObject> wordButtonList = new List<GameObject>();

    void Start()
    {
        ProductParent parentPanel = GetComponent<Panel>().GetProductParent();

        ScientificButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        NormalButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        forChildrenButton = Instantiate(wordButtonPrefab, Buttons.position, Buttons.rotation, Buttons);
        SetButtons(prompt);
        parentPanel.UpdateTheme();
    }

    public void SetButtons(string prompt)
    {
        int lastSelectedTranslationStyleIndex = PlayerPrefs.GetInt("TranslationStyleIndex");

        ScientificButton.GetComponentInChildren<TextMeshProUGUI>().text = "Scientific";
        ScientificButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        ScientificButton.GetComponent<WordButton>().setPromt(prompt);
        ScientificButton.GetComponent<WordButton>().setPromptSentence(Scientific);
        ScientificButton.GetComponent<WordButton>().id = 1;
        if (lastSelectedTranslationStyleIndex == 1) ScientificButton.GetComponent<WordButton>().setActiveIndicator(true);
        wordButtonList.Add(ScientificButton);

        NormalButton.GetComponentInChildren<TextMeshProUGUI>().text = "Normal";
        NormalButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        NormalButton.GetComponent<WordButton>().setPromt(prompt);
        NormalButton.GetComponent<WordButton>().setPromptSentence(Normal);
        NormalButton.GetComponent<WordButton>().id = 2;
        if (lastSelectedTranslationStyleIndex == 2) NormalButton.GetComponent<WordButton>().setActiveIndicator(true);
        wordButtonList.Add(NormalButton);


        forChildrenButton.GetComponentInChildren<TextMeshProUGUI>().text = "Simplified";
        forChildrenButton.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
        forChildrenButton.GetComponent<WordButton>().setPromt(prompt);
        forChildrenButton.GetComponent<WordButton>().setPromptSentence(forChildren);
        forChildrenButton.GetComponent<WordButton>().id = 3;
        if (lastSelectedTranslationStyleIndex == 3) forChildrenButton.GetComponent<WordButton>().setActiveIndicator(true);
        wordButtonList.Add(forChildrenButton);

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