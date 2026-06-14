using UnityEngine;
using UnityEngine.SceneManagement;

// Ana menu mantigi. Once mod (Copenhagen 11 / Tablut 9) ve zorluk secilir,
// sonra oyun sahnesi yuklenir. Secimler GameSettings'e yazilir.
public class MenuController : MonoBehaviour
{
    [Tooltip("Oyun sahnesinin tam adi (Build Settings'teki isim)")]
    public string gameSceneName = "SampleScene";

    // === MOD SECIMI ===
    // Bu butonlar sadece GameSettings'i ayarlar, sahne yuklemez.
    // (Oyuncu once mod, sonra zorluk secer; ya da tersi.)
    public void SelectCopenhagen()
    {
        GameSettings.BoardSize = 11;
    }

    public void SelectTablut()
    {
        GameSettings.BoardSize = 9;
    }

    // === ZORLUK + BASLAT ===
    // Zorluk butonu hem zorlugu ayarlar hem oyunu baslatir.
    public void PlayEasy()
    {
        GameSettings.SetEasy();
        LoadGame();
    }

    public void PlayMedium()
    {
        GameSettings.SetMedium();
        LoadGame();
    }

    public void PlayHard()
    {
        GameSettings.SetHard();
        LoadGame();
    }

    void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}