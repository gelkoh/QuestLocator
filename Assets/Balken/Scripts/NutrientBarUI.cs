using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class NutrientBarUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text nameText;
    public TMP_Text wertText;
    public Image fillImage;

    private RectTransform rt;
    private CanvasGroup canvasGroup;

    private string naehrstoffName;
    private float aktuellerWert;
    private float tagesMax;
    private string einheit;

    private bool datenVorhanden = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        nameText.text = "";
        wertText.text = "";

        canvasGroup.alpha = 0f;
        rt.localScale = Vector3.zero;

        // Starte nur, wenn vorher SetData() aufgerufen wurde
        if (datenVorhanden)
            StarteAnimation();
    }

    public void SetData(string name, float wert, float max, string einheit)
    {
        this.naehrstoffName = name;
        this.aktuellerWert = wert;
        this.tagesMax = max;
        this.einheit = einheit;
        this.datenVorhanden = true;
    }

    void StarteAnimation()
    {
        float prozent = Mathf.Clamp01(aktuellerWert / tagesMax);

        nameText.text = naehrstoffName;
        wertText.text = $"{aktuellerWert:F1} {einheit}";

        slider.maxValue = 1f;
        slider.value = 0f;

        Sequence intro = DOTween.Sequence();
        intro.Append(rt.DOScale(1f, 0.6f).SetEase(Ease.OutBack));
        intro.Join(canvasGroup.DOFade(1f, 0.6f));
        intro.AppendCallback(() =>
        {
            DOTween.To(() => slider.value, x => {
                slider.value = x;
                UpdateFarbe(x);
            }, prozent, 1.5f).SetEase(Ease.InOutQuad);
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

        fillImage.color = zielFarbe;
    }
}
