using Meta.WitAi;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private ProductParent parentPanel;
    [SerializeField] TextMeshProUGUI titleSection;
    bool toggleNutrients = false;
    bool toggleZutaten = false;
    bool toggleUmwelt = false;
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
        if (toggleNutrients == false)
        {
            parentPanel.SetUpNutritionPanel();
            toggleNutrients = true;
        }
        else
        {
            parentPanel.GetNutriPanel().DestroySafely();
            toggleNutrients = false;
        }
    }

    public void spawnZutatenPanel()
    {
        if (toggleZutaten == false)
        {
            parentPanel.SetUpZutatenPanel();
            toggleZutaten = true;
        }
        else
        {
            parentPanel.GetZutantenPanel().DestroySafely();
            toggleZutaten = false;
        }
        
    }

    public void spawnUmweltPanel()
    {
        if (toggleUmwelt == false)
        {
            parentPanel.SetUpFootprintPanel();
            toggleUmwelt = true;
        }
        else
        {
            parentPanel.GetUmweltPanel().DestroySafely();
            toggleUmwelt = false;
        }
    }

    public TextMeshProUGUI getTitleSection()
    {
        return titleSection;
    }
}
