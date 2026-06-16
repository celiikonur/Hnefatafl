using UnityEngine;
using UnityEngine.UI;

// Standart Unity Toggle'ina iOS tarzi "kayan anahtar" gorunumu verir.
// Knob (yuvarlak) On'da saga, Off'ta sola kayar; arka plan rengi degisir.
// Bu script Toggle ile AYNI objeye eklenir.
[RequireComponent(typeof(Toggle))]
public class ToggleSwitch : MonoBehaviour
{
    [Header("Referanslar")]
    public RectTransform knob;        // kayan yuvarlak
    public Image background;          // anahtarin arka plani

    [Header("Konumlar (knob'un local X)")]
    public float offX = -25f;         // sol (Off)
    public float onX = 25f;           // sag (On)

    [Header("Renkler")]
    public Color offColor = new Color(0.4f, 0.4f, 0.4f);
    public Color onColor = new Color(0.3f, 0.7f, 0.4f);

    [Header("Animasyon")]
    public float speed = 12f;

    Toggle toggle;
    float targetX;
    Color targetColor;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnChanged);
        // Baslangic durumunu aninda uygula
        ApplyInstant(toggle.isOn);
    }

    void OnChanged(bool isOn)
    {
        targetX = isOn ? onX : offX;
        targetColor = isOn ? onColor : offColor;
    }

    void ApplyInstant(bool isOn)
    {
        targetX = isOn ? onX : offX;
        targetColor = isOn ? onColor : offColor;

        if (knob != null)
        {
            Vector2 p = knob.anchoredPosition;
            p.x = targetX;
            knob.anchoredPosition = p;
        }
        if (background != null)
            background.color = targetColor;
    }

    void Update()
    {
        // Yumusak kayma
        if (knob != null)
        {
            Vector2 p = knob.anchoredPosition;
            p.x = Mathf.Lerp(p.x, targetX, Time.unscaledDeltaTime * speed);
            knob.anchoredPosition = p;
        }
        if (background != null)
            background.color = Color.Lerp(background.color, targetColor, Time.unscaledDeltaTime * speed);
    }
}