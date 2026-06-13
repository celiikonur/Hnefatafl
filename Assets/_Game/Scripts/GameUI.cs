using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Oyun sonu panelini yonetir. Her frame oyunun bitip bitmedigini kontrol eder,
// bittiyse paneli gosterir ve kazanani yazar. "Tekrar Oyna" sahneyi yeniden yukler.
public class GameUI : MonoBehaviour
{
    public PieceSpawner spawner;
    public GameObject gameOverPanel; // basta gizli, oyun bitince acilir
    public TMP_Text resultText;      // "Saldirganlar Kazandi!" vb.

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

    // Tekrar Oyna butonu bunu cagiracak
    public void Restart()
    {
        // Aktif sahneyi yeniden yukle: her sey Start()'ta sifirdan kurulur
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}