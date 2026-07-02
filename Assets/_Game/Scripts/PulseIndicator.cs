using UnityEngine;

// Tas altindaki gosterge diskini "nabiz gibi" atirir:
// yumusakca buyuyup kuculur ve saydamligi degisir.
// Disk prefab'inin koklerine eklenir.
public class PulseIndicator : MonoBehaviour
{
    [Header("Nabiz ayarlari")]
    [Tooltip("Saniyede kac nabiz")]
    public float speed = 1.5f;

    [Tooltip("En kucuk olcek")]
    public float minScale = 0.8f;
    [Tooltip("En buyuk olcek")]
    public float maxScale = 1.1f;

    [Header("Saydamlik (opsiyonel)")]
    [Tooltip("Acikken saydamligi da nabizla degistirir")]
    public bool pulseAlpha = true;
    public float minAlpha = 0.35f;
    public float maxAlpha = 0.85f;

    Vector3 baseScale;
    Renderer rend;
    MaterialPropertyBlock mpb;
    Color baseColor = Color.white;
    float phase;

    void Awake()
    {
        baseScale = transform.localScale;

        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<Renderer>();

        if (rend != null && pulseAlpha)
        {
            mpb = new MaterialPropertyBlock();
            // Baslangic rengini materyalden al (URP/Lit: _BaseColor)
            if (rend.sharedMaterial != null && rend.sharedMaterial.HasProperty("_BaseColor"))
                baseColor = rend.sharedMaterial.GetColor("_BaseColor");
        }

        // Her gosterge biraz farkli fazda basiasin ki hepsi ayni anda atmasin
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed + phase) + 1f) * 0.5f; // 0..1

        // Olcek nabzi
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = baseScale * s;

        // Saydamlik nabzi
        if (pulseAlpha && rend != null && mpb != null)
        {
            float a = Mathf.Lerp(minAlpha, maxAlpha, t);
            Color c = baseColor;
            c.a = a;
            rend.GetPropertyBlock(mpb);
            if (rend.sharedMaterial != null && rend.sharedMaterial.HasProperty("_BaseColor"))
                mpb.SetColor("_BaseColor", c);
            rend.SetPropertyBlock(mpb);
        }
    }
}