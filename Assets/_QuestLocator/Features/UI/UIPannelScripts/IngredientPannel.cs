using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
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

        int id = 0;
        foreach (var ingredient in productDisplayScript.productData.Product.Ingredients)
        {
            GameObject wordButtonInstance = Instantiate(wordButtonPrefab, ingredientTransform);
            wordButtonInstance.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.Text;
            wordButtonInstance.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
            wordButtonInstance.GetComponent<WordButton>().setPromt(ingredient.Id[3..]);
            wordButtonInstance.GetComponent<WordButton>().setPromptSentence(" auf einfache, kurze aber präzise Weise, sodass jeder die grundlegende Funktion oder Bedeutung versteht. Der Text soll ungestyled sein");
            wordButtonInstance.GetComponent<WordButton>().id = id;
            wordButtonList.Add(wordButtonInstance);
            id++;
        }


    }

    public List<GameObject> GetWordList()
    {
        return wordButtonList;
    }
}

