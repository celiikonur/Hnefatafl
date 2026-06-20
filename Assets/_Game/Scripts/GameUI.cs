using UnityEngine;
using UnityEngine.SceneManagement;

// Oyun sonu ekrani. Oyun bitince:
//  1) Viking'in tepki animasyonunu tetikler:
//       - Rakip (Viking) kazandiysa  -> Win  (sevinme)
//       - Rakip (Viking) kaybettiyse -> Lost (uzulme)
//  2) Gecikme sonrasi sonuc panelini acar (Victory/Defeated/Draw)
public class GameUI : MonoBehaviour
{
    [Header("Referanslar")]
    public PieceSpawner spawner;
    public GameObject gameOverPanel;

    [Header("Viking Animasyon")]
    public Animator vikingAnimator;

    [Header("Sonuc Gorselleri (panelde, baslangicta kapali)")]
    public GameObject victoryImage;   // oyuncu kazandi
    public GameObject defeatedImage;  // oyuncu kaybetti
    public GameObject drawImage;      // berabere

    [Header("Sahne")]
    public string menuSceneName = "MainMenu";

    [Header("Gecikme")]
    public float showDelay = 2.5f;

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

        if (!triggered && spawner.State.GameOver)
        {
            triggered = true;
            timer = 0f;
            TriggerVikingReaction();
        }

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

    // Viking = rakip. Sonuca gore sevinir ya da uzulur.
    void TriggerVikingReaction()
    {
        if (vikingAnimator == null) return;
        if (spawner.State.IsDraw) return; // berabere: tepki yok

        bool playerWon = (spawner.State.AttackerWon == GameSettings.PlayerIsAttacker);

        if (!playerWon)
            vikingAnimator.SetTrigger("Win");   // rakip kazandi -> sevinir
        else
            vikingAnimator.SetTrigger("Lose");  // rakip kaybetti -> uzulur
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