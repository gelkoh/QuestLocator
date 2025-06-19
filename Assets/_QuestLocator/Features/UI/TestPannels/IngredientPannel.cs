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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        productDisplayScript = GetComponentInParent<ProductParent>();
        content = zutatenListe.GetNamedChild("Content");
        ingredientTransform = content.transform;
        FillInfo();
    }

    private void FillInfo()
    {
        title.text = productDisplayScript.productData.Product.ProductName;

        foreach (var ingredient in productDisplayScript.productData.Product.Ingredients)
        {
            GameObject wordButtonInstance = Instantiate(wordButtonPrefab, ingredientTransform);
            wordButtonInstance.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.Text;
            wordButtonInstance.GetComponent<WordButton>().SetParentPanel(this.GetComponent<Panel>());
            wordButtonInstance.GetComponent<WordButton>().setPromt(ingredient.Text);
            wordButtonInstance.GetComponent<WordButton>().setPromptSentence(" auf einfache, kurze aber präzise Weise, sodass jeder die grundlegende Funktion oder Bedeutung versteht.");
            wordButtonList.Add(wordButtonInstance);
            Debug.Log(zutatenListe.GetComponent<RectTransform>().sizeDelta);
        }

        
    }
}

