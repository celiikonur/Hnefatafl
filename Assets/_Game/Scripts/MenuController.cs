using UnityEngine;
using UnityEngine.SceneManagement;

// Ana menu mantigi. Zorluk butonlari secimi GameSettings'e yazar,
// sonra oyun sahnesini yukler.
public class MenuController : MonoBehaviour
{
    [Tooltip("Oyun sahnesinin tam adi (Build Settings'teki isim)")]
    public string gameSceneName = "SampleScene";

    // === Zorluk butonlari ===
    // Her biri bir Button'in OnClick'ine baglanacak.

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

    // Cikis butonu (opsiyonel; editorde ise yaramaz, build'de calisir)
    public void QuitGame()
    {
        Application.Quit();
    }
}