using Meta.WitAi;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    private ProductParent parentPanel;
    [SerializeField] TextMeshProUGUI titleSection;
    [SerializeField] Image nutrientsIcon;
    [SerializeField] Sprite nOpen;
    [SerializeField] Sprite nClose;
    [SerializeField] Image zutantenIcon;
    [SerializeField] Sprite zOpen;
    [SerializeField] Sprite zClose;
    [SerializeField] Image umweltIcon;
    [SerializeField] Sprite uOpen;
    [SerializeField] Sprite uClose;
    public bool NutrientsIsActive = true;
    public bool ZutatenIsActive = false;
    public bool UmweltIsACtive = false;
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
        if (NutrientsIsActive == false)
        {
            Debug.LogError("create");
            nutrientsIcon.sprite = nClose;
            parentPanel.SetUpNutritionPanel();
            NutrientsIsActive = true;
            Debug.LogError("after");
        }
        else
        {
            Debug.LogError("close");
            nutrientsIcon.sprite = nOpen;
            parentPanel.GetNutriPanel().DestroySafely();
            NutrientsIsActive = false;
            Debug.LogError("after");
        }
    }

    public void spawnZutatenPanel()
    {
        if (ZutatenIsActive == false)
        {
            zutantenIcon.sprite = zClose;
            parentPanel.SetUpZutatenPanel();
            ZutatenIsActive = true;
        }
        else
        {
            zutantenIcon.sprite = zOpen;
            parentPanel.GetZutantenPanel().DestroySafely();
            ZutatenIsActive = false;
        }

    }

    public void spawnUmweltPanel()
    {
        if (UmweltIsACtive == false)
        {
            umweltIcon.sprite = uClose;
            parentPanel.SetUpFootprintPanel();
            UmweltIsACtive = true;
        }
        else
        {
            umweltIcon.sprite = uOpen;
            parentPanel.GetUmweltPanel().DestroySafely();
            UmweltIsACtive = false;
        }
    }

    public TextMeshProUGUI getTitleSection()
    {
        return titleSection;
    }
}
