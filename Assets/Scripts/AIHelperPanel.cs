using TMPro;
using UnityEngine;

public class AIHelperPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextSection;
    [SerializeField] TextMeshProUGUI Title;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI GetTextSection()
    {
        return TextSection;
    }

    public TextMeshProUGUI GetTitle()
    {
        return Title;
    }

    public void DestroyObj()
    {
        Destroy(gameObject); 
    }
}
