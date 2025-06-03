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
    List<GameObject> wordButtonList = new List<GameObject>();

    
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
                wordButtonList.Add(wordButtonInstance);
                }

            }
        }
    }
    
}
