using UnityEngine;

// Zorluk seviyesine gore dogru Viking modelini aktif eder, digerlerini kapatir.
// Uc Viking de sahnede ayni yerde durur (sandalyede); bu script sadece
// hangisinin gorunecegine karar verir.
//
// GameSettings.DifficultyLevel: 0 = Kolay, 1 = Orta, 2 = Zor
public class VikingSelector : MonoBehaviour
{
    [Header("Zorluga gore Viking modelleri")]
    [Tooltip("Kolay (Genc Viking)")]
    public GameObject easyViking;

    [Tooltip("Orta (Savasci)")]
    public GameObject mediumViking;

    [Tooltip("Zor (Reis)")]
    public GameObject hardViking;

    [Header("Animasyon")]
    [Tooltip("Secilen Viking'in Animator'i GameUI'a otomatik baglanir")]
    public GameUI gameUI;

    void Awake()
    {
        // Once hepsini kapat
        if (easyViking != null) easyViking.SetActive(false);
        if (mediumViking != null) mediumViking.SetActive(false);
        if (hardViking != null) hardViking.SetActive(false);

        // Zorluga gore dogru olani sec
        GameObject selected = null;
        switch (GameSettings.DifficultyLevel)
        {
            case 0: selected = easyViking; break;
            case 1: selected = mediumViking; break;
            case 2: selected = hardViking; break;
            default: selected = mediumViking; break;
        }

        if (selected != null)
        {
            selected.SetActive(true);

            // Secilen Viking'in Animator'ini GameUI'a bagla (tepki animasyonlari icin)
            if (gameUI != null)
            {
                Animator anim = selected.GetComponent<Animator>();
                if (anim == null) anim = selected.GetComponentInChildren<Animator>();
                if (anim != null) gameUI.vikingAnimator = anim;
            }
        }
    }
}