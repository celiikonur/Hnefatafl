using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Oyun sonu panelini yonetir. Oyun bitince paneli gosterir, kazanani yazar.
// "Tekrar Oyna" sahneyi yeniden yukler; "Ana Menu" menu sahnesine doner.
public class GameUI : MonoBehaviour
{
    public PieceSpawner spawner;
    public GameObject gameOverPanel;
    public TMP_Text resultText;

    [Tooltip("Ana menu sahnesinin tam adi (Build Settings'teki isim)")]
    public string menuSceneName = "MainMenu";

    bool shown;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (shown) return;
        if (spawner == null || spawner.State == null) return;

        if (spawner.State.GameOver)
            ShowResult();
    }

    void ShowResult()
    {
        shown = true;

        if (resultText != null)
        {
            resultText.text = spawner.State.AttackerWon
                ? "Saldirganlar Kazandi!"
                : "Savunmacilar Kazandi!";
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // Tekrar Oyna butonu
    public void Restart()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    // Ana Menu butonu
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}