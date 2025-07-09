using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Android;

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

    [SerializeField] TextMeshProUGUI Allergies;
    [SerializeField] GameObject VeganIcon;
    [SerializeField] GameObject VegetarianIcon;

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

        int id = 0;
        foreach (var ingredient in productDisplayScript.productData.Product.Ingredients)
        {
            GameObject wordButtonInstance = Instantiate(wordButtonPrefab, ingredientTransform);
            wordButtonInstance.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.Id[3..];
            wordButtonInstance.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());

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

            wordButtonInstance.GetComponent<WordButton>().setPromt(ingredient.Id[3..]);
            wordButtonInstance.GetComponent<WordButton>().id = id;

            wordButtonList.Add(wordButtonInstance);
            id++;
        }
        Debug.LogError("beforeTags");
        if (productDisplayScript.productData.Product.AllergensTags.Length > 0)
        {
            Debug.LogError("In allergens");
            String text = "";
            for (int i = 0; i < productDisplayScript.productData.Product.AllergensTags.Length; i++)
            {
                text += productDisplayScript.productData.Product.AllergensTags[i][3..] + ", ";
            }
            Allergies.text = text;
        }

        if (productDisplayScript.productData.Product.IngredientsAnalysisTags != null)
        {
            Debug.LogError("in Veg");
            foreach (string tag in productDisplayScript.productData.Product.IngredientsAnalysisTags)
            {
                if (tag.Equals("en:vegan", StringComparison.OrdinalIgnoreCase))
                {
                    VeganIcon.SetActive(true);
                    return; // Exit early since we found the tag
                }
                if (tag.Equals("en:vegetarian", StringComparison.OrdinalIgnoreCase))
                {
                    VegetarianIcon.SetActive(true);
                    return; // Exit early since we found the tag
                }
            }
    }
    }

    public List<GameObject> GetWordList()
    {
        return wordButtonList;
    }
}

