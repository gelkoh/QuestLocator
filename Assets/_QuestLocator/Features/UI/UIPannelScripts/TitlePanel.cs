using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private ProductParent parentPanel;
    [SerializeField] TextMeshProUGUI titleSection;
    void Start()
    {
        parentPanel = GetComponent<Panel>().GetProductParent();
    }

    public void CenterPanels()
    {
        if (parentPanel.GetZutantenPanel() != null)
        {
            parentPanel.GetZutantenPanel().GetComponent<Panel>().setCanvasPosition(Vector3.zero);
            parentPanel.GetZutantenPanel().GetComponent<Panel>().setCanvasRotation(parentPanel.GetContainer().eulerAngles);
        }

        if (parentPanel.GetUmweltPanel() != null)
        {
            parentPanel.GetUmweltPanel().GetComponent<Panel>().setCanvasPosition(Vector3.zero);
            parentPanel.GetUmweltPanel().GetComponent<Panel>().setCanvasRotation(parentPanel.GetContainer().eulerAngles);
        }

        if (parentPanel.GetNutriPanel() != null)
        {
            parentPanel.GetNutriPanel().GetComponent<Panel>().setCanvasPosition(Vector3.zero);
            parentPanel.GetNutriPanel().GetComponent<Panel>().setCanvasRotation(parentPanel.GetContainer().eulerAngles);
        }

        if (parentPanel.GetGeminiPanel() != null)
        {
            parentPanel.GetGeminiPanel().GetComponent<Panel>().setCanvasPosition(Vector3.zero);
            parentPanel.GetGeminiPanel().GetComponent<Panel>().setCanvasRotation(parentPanel.GetContainer().eulerAngles);
        }
        this.GetComponent<Panel>().setCanvasPosition(Vector3.zero);
        this.GetComponent<Panel>().setCanvasRotation(parentPanel.GetContainer().eulerAngles);
    }
    public void spawnNutrientsPanel()
    {
        parentPanel.SetUpNutritionPanel();
    }

    public void spawnZutatenPanel()
    {
        parentPanel.SetUpZutatenPanel();
    }

    public void spawnUmweltPanel()
    {
        parentPanel.SetUpFootprintPanel();
    }

    public TextMeshProUGUI getTitleSection()
    {
        return titleSection;
    }
}
