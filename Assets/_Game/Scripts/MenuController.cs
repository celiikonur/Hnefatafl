using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Ana menu mantigi. Mod ve taraf secimi TOGGLE ile yapilir (iOS tarzi anahtar).
// Zorluk butonu oyunu baslatir. Secimler GameSettings'e yazilir.
public class MenuController : MonoBehaviour
{
    [Tooltip("Oyun sahnesinin tam adi (Build Settings'teki isim)")]
    public string gameSceneName = "SampleScene";

    // === TOGGLE FONKSIYONLARI ===
    // Toggle'in OnValueChanged olayina baglanir; isOn parametresini Unity otomatik gonderir.

    // Mod toggle: ON = Tablut (9), OFF = Copenhagen (11)
    public void OnModeToggle(bool isOn)
    {
        GameSettings.BoardSize = isOn ? 9 : 11;
    }

    // Taraf toggle: ON = Savunmaci, OFF = Saldirgan
    public void OnSideToggle(bool isOn)
    {
        GameSettings.PlayerIsAttacker = !isOn;
    }

    // === ZORLUK + BASLAT ===
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