using System;
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
    private String prompt;
    String wissenschaftlich = " präzise und wissenschaftlich korrekt in maximal zwei Sätzen. Verwende dabei die Terminologie der jeweiligen Fachdisziplin.";
    String einfach = " so, dass ihn auch jemand ohne Vorwissen versteht. Die Erklärung soll korrekt, einfach und höchstens zwei kurze Sätze lang sein.";
    String fuerKinder = "auf einfache, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht. In Maximal 2 Sätzen.";

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
    
}
