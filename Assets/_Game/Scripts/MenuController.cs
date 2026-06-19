using UnityEngine;
using UnityEngine.SceneManagement;

// Ana menu mantigi. Iki panel arasinda gecis yapar:
//  - mainPanel: Logo + PLAY + Nasil Oynanir + Ayarlar + Cikis
//  - setupPanel: Mod/Taraf toggle + Zorluk butonlari + Geri
// Ayarlar ve Nasil Oynanir panelleri simdilik bos iskelet (sonra doldurulacak).
public class MenuController : MonoBehaviour
{
    [Header("Sahne")]
    [Tooltip("Oyun sahnesinin tam adi (Build Settings'teki isim)")]
    public string gameSceneName = "SampleScene";

    [Header("Paneller")]
    public GameObject mainPanel;        // ana menu
    public GameObject setupPanel;       // oyun ayarlari (mod/taraf/zorluk)
    public GameObject settingsPanel;    // ayarlar (simdilik bos)
    public GameObject howToPlayPanel;   // nasil oynanir (simdilik bos)

    void Start()
    {
        // Baslangicta sadece ana menu acik
        ShowMainPanel();
    }

    // === ANA MENU BUTONLARI ===

    // PLAY -> oyun ayarlari panelini ac
    public void OnPlayButton()
    {
        HideAll();
        if (setupPanel != null) setupPanel.SetActive(true);
    }

    public void OnHowToPlayButton()
    {
        HideAll();
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void OnSettingsButton()
    {
        HideAll();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    // === GERI BUTONU (alt panellerden ana menuye) ===
    public void ShowMainPanel()
    {
        HideAll();
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    void HideAll()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (setupPanel != null) setupPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    // === OYUN AYARLARI PANELI ===

    // Mod toggle: ON = Tablut (9), OFF = Copenhagen (11)
    public void OnModeToggle(bool isOn)
{
    GameSettings.BoardSize = isOn ? 11 : 9;
}

public void OnSideToggle(bool isOn)
{
    GameSettings.PlayerIsAttacker = isOn;
}

    // Zorluk butonlari -> oyunu baslatir
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
}