using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class NutrientBarUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text nameText;
    public TMP_Text wertText;
    public TMP_Text tagesbedarfText;
    public Image fillImage;

    private RectTransform rt;
    private CanvasGroup canvasGroup;

    private string naehrstoffName;
    private float aktuellerWert;
    private float tagesMax;
    private string einheit;

    private bool datenGesetzt = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Setze Standard-Anfangswerte für die UI
        if (nameText != null) nameText.text = "";
        //if (wertText != null) wertText.text = "";
        wertText.text = "";
        if (slider != null) slider.value = 0f;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (rt != null) rt.localScale = Vector3.zero;
    }

    public void SetData(string name, float wert, float max, string einheit)
    {
        this.naehrstoffName = name;
        this.aktuellerWert = wert;
        this.tagesMax = max;
        this.einheit = einheit;
        this.datenGesetzt = true;

        if (name != "Energy")
        {
            tagesbedarfText.SetText($"{tagesMax:0} g");
        }
        else
        {
            tagesbedarfText.SetText($"{tagesMax:0} kcal");
        }
    }

    public void InitializeAndAnimate()
    {
        if (!datenGesetzt)
        {
            Debug.LogWarning($"[NutrientBarUI] Attempted to animate bar '{naehrstoffName}', but no data was set yet. Call SetData() first.");
            return;
        }

        float prozent = Mathf.Clamp01(aktuellerWert / tagesMax);

        // Sicherstellen, dass Textfelder und Slider existieren, bevor darauf zugegriffen wird
        if (nameText != null) nameText.text = naehrstoffName +"  "+ $"{aktuellerWert:F1} {einheit}";
        //if (wertText != null) wertText.text = $"{aktuellerWert:F1} {einheit}";
        
        if (slider != null)
        {
            slider.maxValue = 1f;
            slider.value = 0f;
        }
        else
        {
            Debug.LogError("[NutrientBarUI] Slider not assigned for bar: " + naehrstoffName);
            return;
        }

        Sequence intro = DOTween.Sequence();
        if (rt != null) intro.Append(rt.DOScale(1f, 0.6f).SetEase(Ease.OutBack));
        if (canvasGroup != null) intro.Join(canvasGroup.DOFade(1f, 0.6f));
        intro.AppendCallback(() =>
        {
            if (slider != null)
            {
                DOTween.To(() => slider.value, x => {
                    slider.value = x;
                    UpdateFarbe(x);
                }, prozent, 1.5f).SetEase(Ease.InOutQuad);
            }
        });
    }

    void UpdateFarbe(float prozent)
    {
        Color gruen = Color.green;
        Color gelb = Color.yellow;
        Color rot = Color.red;
        Color zielFarbe;

        if (prozent < 0.5f)
        {
            float t = prozent / 0.5f;
            zielFarbe = Color.Lerp(gruen, gelb, t);
        }
        else
        {
            float t = (prozent - 0.5f) / 0.5f;
            zielFarbe = Color.Lerp(gelb, rot, t);
        }

        if (fillImage != null) fillImage.color = zielFarbe;
    }
}
