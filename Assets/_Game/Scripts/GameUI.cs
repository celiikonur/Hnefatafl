using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
            if (spawner.State.IsDraw)
                resultText.text = "Berabere!";
            else
                resultText.text = spawner.State.AttackerWon
                    ? "Saldirganlar Kazandi!"
                    : "Savunmacilar Kazandi!";
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}