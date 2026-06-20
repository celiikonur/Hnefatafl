using UnityEngine;
using UnityEngine.SceneManagement;

// Oyun sonu ekrani. Oyun bitince paneli acar ve oyuncunun sonucuna gore
// uc gorselden dogru olani gosterir: Victory / Defeated / Draw.
public class GameUI : MonoBehaviour
{
    [Header("Referanslar")]
    public PieceSpawner spawner;
    public GameObject gameOverPanel;

    [Header("Sonuc Gorselleri (panelde, baslangicta kapali)")]
    public GameObject victoryImage;   // oyuncu kazandi
    public GameObject defeatedImage;  // oyuncu kaybetti
    public GameObject drawImage;      // berabere

    [Header("Sahne")]
    public string menuSceneName = "MainMenu";

    [Header("Gecikme")]
    [Tooltip("Son hamle gorulduken kac saniye sonra panel acilsin")]
    public float showDelay = 2f;

    bool triggered;
    bool shown;
    float timer;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        HideAllResults();
    }

    void Update()
    {
        if (shown) return;
        if (spawner == null || spawner.State == null) return;

        // 1) Oyun bitti mi? (henuz panel acma, sadece sayaci baslat)
        if (!triggered && spawner.State.GameOver)
        {
            triggered = true;
            timer = 0f;

            // TESHIS: gercek degerleri yazdir
            Debug.Log($"[GAMEOVER] IsDraw={spawner.State.IsDraw} | AttackerWon={spawner.State.AttackerWon} | PlayerIsAttacker={GameSettings.PlayerIsAttacker}");
        }

        // 2) Tetiklendiyse, gecikme dolunca paneli ac
        if (triggered && !shown)
        {
            timer += Time.deltaTime;
            if (timer >= showDelay)
            {
                shown = true;
                ShowResult();
            }
        }
    }

    void ShowResult()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        HideAllResults();

        if (spawner.State.IsDraw)
        {
            if (drawImage != null) drawImage.SetActive(true);
            return;
        }

        bool playerWon = (spawner.State.AttackerWon == GameSettings.PlayerIsAttacker);

        if (playerWon)
        {
            if (victoryImage != null) victoryImage.SetActive(true);
        }
        else
        {
            if (defeatedImage != null) defeatedImage.SetActive(true);
        }
    }

    void HideAllResults()
    {
        if (victoryImage != null) victoryImage.SetActive(false);
        if (defeatedImage != null) defeatedImage.SetActive(false);
        if (drawImage != null) drawImage.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}