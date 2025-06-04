using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class KalorienBalkenController : MonoBehaviour
{
    [System.Serializable]
    public class KalorienDaten
    {
        public string name;
        public float wert;
        public float max;
        public string einheit;
    }

    public TextAsset jsonDatei;
    public Slider slider;
    public TMP_Text nameText;
    public TMP_Text wertText;
    public Image fillImage;

    private RectTransform rt;
    private CanvasGroup canvasGroup;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        nameText.text = "";
        wertText.text = "";

        canvasGroup.alpha = 0f;
        rt.localScale = Vector3.zero;

        Sequence intro = DOTween.Sequence();
        intro.Append(rt.DOScale(1f, 0.6f).SetEase(Ease.OutBack));
        intro.Join(canvasGroup.DOFade(1f, 0.6f));
        intro.AppendCallback(StarteAnimation);
    }

    void StarteAnimation()
    {
        if (jsonDatei == null) return;

        KalorienDaten daten = JsonUtility.FromJson<KalorienDaten>(jsonDatei.text);
        float prozent = daten.wert / daten.max;

        nameText.text = daten.name;
        wertText.text = $"{daten.wert} {daten.einheit}";

        slider.maxValue = 1f;
        slider.value = 0f;

        // DOTween-Füllung + Farbübergang synchron
        DOTween.To(() => slider.value, x => {
            slider.value = x;
            UpdateFarbe(x);
        }, prozent, 1.5f).SetEase(Ease.InOutQuad);
    }

    void UpdateFarbe(float prozent)
    {
        // Farbverlauf: grün → gelb → rot
        Color gruen = Color.green;
        Color gelb = Color.yellow;
        Color rot = Color.red;
        Color zielFarbe;

        if (prozent < 0.5f)
        {
            // 0.0 – 0.5 = grün → gelb
            float t = prozent / 0.5f;
            zielFarbe = Color.Lerp(gruen, gelb, t);
        }
        else
        {
            // 0.5 – 1.0 = gelb → rot
            float t = (prozent - 0.5f) / 0.5f;
            zielFarbe = Color.Lerp(gelb, rot, t);
        }

        fillImage.color = zielFarbe;
    }
}
