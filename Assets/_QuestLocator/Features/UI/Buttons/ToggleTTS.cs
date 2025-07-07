using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTTS : MonoBehaviour
{
    private GameObject TTS;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unmuteSprite;
    [SerializeField] private TextMeshProUGUI text;
    private bool muted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TTS = GameObject.FindGameObjectWithTag("TTS");
    }

    public void Toggle()
    {
        if (muted == false)
        {
            icon.sprite = mutedSprite;
            text.text = "mute";
            TTS.SetActive(true);
            muted = true;
        }
        else
        {
            icon.sprite = unmuteSprite;
            text.text = "unmute";
            TTS.SetActive(false);
            muted = false;
        }
    }
}
