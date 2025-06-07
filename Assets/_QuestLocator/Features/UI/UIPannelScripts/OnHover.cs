using System.Collections;
using UnityEngine;

public class OnHover : MonoBehaviour
{
    [SerializeField] GameObject gap;
    [SerializeField] GameObject text;
    bool hovering = false;
   
    IEnumerator expand()
    {
        yield return new WaitForSeconds(1);
        if (hovering)
        {
            gap.SetActive(true);
            text.SetActive(true);
        }
        
    }

    void collaps()
    {
        gap.gameObject.SetActive(false);
        text.SetActive(false);
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
