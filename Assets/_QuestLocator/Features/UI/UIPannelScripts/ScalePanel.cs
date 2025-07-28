using UnityEngine;
using UnityEngine.UI;

public class ScalePanel : MonoBehaviour
{
    Vector2 orgSize;

    void Start()
    {
        orgSize = this.GetComponent<RectTransform>().sizeDelta;

    }

    public void reduceSize(GameObject obj)
    {
        if (obj != null && obj.GetComponent<RectTransform>() != null)
        {
            float width = obj.GetComponent<RectTransform>().sizeDelta.x;
            float padding = this.GetComponent<HorizontalLayoutGroup>().padding.left + this.GetComponent<HorizontalLayoutGroup>().padding.right;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(width + padding, orgSize.y);
        }
        else
        {
            Debug.LogWarning("NO RectTransform Connected");
        }
    }

    public void expandSize()
    {
        this.GetComponent<RectTransform>().sizeDelta = orgSize;
    }
}