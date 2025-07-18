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
    //String Scientific = " präzise und Scientific korrekt in maximal zwei Sätzen. Verwende dabei die Terminologie der jeweiligen Fachdisziplin. Der Text soll ungestyled sein";
    String Scientific = " precisely and scientifically correct with a maximum of 434 characters. Use the terminology of the respective discipline. The text should be unstyled";
    //String Normal = " so, dass ihn auch jemand ohne Vorwissen versteht. Die Erklärung soll korrekt, Normal und höchstens zwei kurze Sätze lang sein. Der Text soll ungestyled sein";
    String Normal = " in such a way that someone without prior knowledge can understand it. The explanation should be correct, normal and no longer than 434 characters. The text should be unstyled";
    //String forChildren = "auf Normale, kurze aber präzise Weise, sodass ein Kind die grundlegende Funktion oder Bedeutung versteht. In Maximal 2 Sätzen. Der Text soll ungestyled sein";
    String forChildren = " in a normal, short but precise way so that a child understands the basic function or meaning. In a maximum of 434 characters. The text should be unstyled";
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
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(Normal);
            }
            else if (lastSelectedTranslationStyleIndex == 1)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(Scientific);
            }
            else if (lastSelectedTranslationStyleIndex == 2)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(Normal);
            }
            else if (lastSelectedTranslationStyleIndex == 3)
            {
                wordButtonInstance.GetComponent<WordButton>().setPromptSentence(forChildren);
            }

            wordButtonInstance.GetComponent<WordButton>().setPromt(ingredient.Id[3..]);
            wordButtonInstance.GetComponent<WordButton>().id = id;

            wordButtonList.Add(wordButtonInstance);
            id++;
        }
        
        if (productDisplayScript.productData.Product.AllergensTags.Length > 0)
        {
            
            String text = "";
            for (int i = 0; i < productDisplayScript.productData.Product.AllergensTags.Length; i++)
            {
                text += productDisplayScript.productData.Product.AllergensTags[i][3..] + ", ";
            }
            Allergies.text = text;
        }

        if (productDisplayScript.productData.Product.IngredientsAnalysisTags != null)
        {
            
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

