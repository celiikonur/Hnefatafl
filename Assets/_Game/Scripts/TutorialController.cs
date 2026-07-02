using UnityEngine;

// "Nasil Oynanir" ekrani. Birden fazla sayfa (panel) arasinda
// Ileri/Geri ile gezinir. Her sayfa ayri bir GameObject (gorsel + metin).
//
// Kurulum:
//  - pages dizisine sayfa GameObject'lerini sirayla ekle (Sayfa1, Sayfa2, Sayfa3)
//  - Ileri butonu -> NextPage
//  - Geri butonu  -> PrevPage
//  - Kapat butonu -> menuController.ShowMainPanel (ana menuye doner)
public class TutorialController : MonoBehaviour
{
    [Header("Sayfalar (sirayla)")]
    public GameObject[] pages;

    [Header("Navigasyon butonlari (opsiyonel)")]
    [Tooltip("Ilk sayfada Geri'yi gizlemek icin")]
    public GameObject prevButton;
    [Tooltip("Son sayfada Ileri'yi gizlemek icin")]
    public GameObject nextButton;

    int currentPage;

    void OnEnable()
    {
        // Panel her acildiginda ilk sayfadan basla
        currentPage = 0;
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    void ShowPage(int index)
    {
        // Sadece secili sayfayi goster
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == index);
        }

        // Ilk sayfada Geri gizli, son sayfada Ileri gizli (opsiyonel)
        if (prevButton != null)
            prevButton.SetActive(index > 0);
        if (nextButton != null)
            nextButton.SetActive(index < pages.Length - 1);
    }
}