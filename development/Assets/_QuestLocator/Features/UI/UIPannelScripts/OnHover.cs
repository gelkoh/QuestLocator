using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OnHover : MonoBehaviour
{
    [SerializeField] GameObject gap;
    [SerializeField] GameObject text;
    [SerializeField] GameObject icon;
    [SerializeField] HorizontalLayoutGroup horizontalLayoutGroup;
    bool hovering = false;

    IEnumerator expand()
    {
        yield return new WaitForSeconds(1);

        if (hovering)
        {
            text.SetActive(true);
            icon.SetActive(false);
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
        }
    }

    void collaps()
    {
        text.SetActive(false);
        icon.gameObject.SetActive(true);
        horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }

    public void isHovering(bool hovering)
    {
        this.hovering = hovering;
        if (hovering)
        {
            StartCoroutine(expand());
        }
        else
        {
            collaps();
        }
    }
}