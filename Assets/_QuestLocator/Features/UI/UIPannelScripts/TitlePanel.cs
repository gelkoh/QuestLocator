using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private ProductParent parentPanel;
    void Start()
    {
        parentPanel = GetComponent<Panel>().GetProductParent();
    }

    public void CenterPanels()
    {
        parentPanel.GetZutantenPanel().transform.position = new Vector3(0,0,0);
        parentPanel.GetUmweltPanel().transform.position = new Vector3(0,0,0);
        parentPanel.GetNutriPanel().transform.position = new Vector3(0,0,0);
        parentPanel.GetGeminiPanel().transform.position = new Vector3(0, 0, 0);
    }
}
