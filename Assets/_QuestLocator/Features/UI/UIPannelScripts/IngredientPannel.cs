using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class IngredientPannel : MonoBehaviour
{
    private ProductParent productDisplayScript;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject wordButtonPrefab;
    Transform ingredientTransform;
    [SerializeField] GameObject content;
    [SerializeField] GameObject zutatenListe;
    List<GameObject> wordButtonList = new List<GameObject>();

    // TODO: Store globally later because it's also used in GeminiPanel.cs
    String wissenschaftlich = " präzise und wissenschaftlich korrekt in maximal zwei Sätzen. Verwende dabei die Terminologie der jeweiligen Fachdisziplin. Der Text soll ungestyled sein";
    String einfach = " so, dass ihn auch jemand ohne Vorwissen versteht. Die Erklärung soll korrekt, einfach und höchstens zwei kurze Sätze lang sein. Der Text soll ungestyled sein";
    String fuerKinder = "auf einfache, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht. In Maximal 2 Sätzen. Der Text soll ungestyled sein";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //productDisplayScript = GetComponentInParent<ProductParent>();
        //content = zutatenListe.GetNamedChild("Content");
        //ingredientTransform = content.transform;
        //FillInfo();
    }

    public void FillInfo()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        content = zutatenListe.GetNamedChild("Content");
        ingredientTransform = content.transform;
        
        title.text = productDisplayScript.productData.Product.ProductName;

        int lastSelectedTranslationStyleIndex = PlayerPrefs.GetInt("TranslationStyleIndex");

        foreach (var ingredient in productDisplayScript.productData.Product.Ingredients)
        {
            GameObject wordButtonInstance = Instantiate(wordButtonPrefab, ingredientTransform);
            wordButtonInstance.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.Text;
            wordButtonInstance.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
            wordButtonInstance.GetComponent<WordButton>().setPromt(ingredient.Text);
            // wordButtonInstance.GetComponent<WordButton>().setPromptSentence(" auf einfache, kurze aber präzise Weise, sodass jeder die grundlegende Funktion oder Bedeutung versteht. Der Text soll ungestyled sein");

            if (lastSelectedTranslationStyleIndex == 0)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(einfach);
            }
            else if (lastSelectedTranslationStyleIndex == 1)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(wissenschaftlich);
            }
            else if (lastSelectedTranslationStyleIndex == 2)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(einfach);
            }
            else if (lastSelectedTranslationStyleIndex == 3)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(fuerKinder);
            }

            wordButtonList.Add(wordButtonInstance);
            Debug.Log(zutatenListe.GetComponent<RectTransform>().sizeDelta);
        }
    }
}

