using UnityEngine;
using TMPro;

// Ekranin ustunde "kimin sirasi" yazisini gosterir ve gunceller.
// Ayrica oyuncunun sirasi geldiginde/gittiginde olay yayinlar
// (tas alti gostergesi gibi seyler bunu dinleyebilir).
public class TurnIndicator : MonoBehaviour
{
    [Header("Referanslar")]
    public PieceSpawner spawner;
    public TMP_Text turnText;

    [Header("Metinler (ABD pazari icin Ingilizce)")]
    public string attackerTurnText = "Attackers' Turn";
    public string defenderTurnText = "Defenders' Turn";
    public string yourTurnText = "Your Turn";
    public string opponentTurnText = "Opponent's Turn";

    [Tooltip("Acikken 'Your Turn/Opponent' gosterir; kapaliyken taraf isimlerini")]
    public bool showPlayerPerspective = true;

    // Oyuncunun sirasi mi? (tas alti gostergesi bunu dinler)
    public bool IsPlayerTurn { get; private set; }

    bool lastWasPlayerTurn;
    bool initialized;

    void Update()
    {
        if (spawner == null || spawner.State == null) return;

        // Oyun bittiyse sira yazisini gizle/temizle
        if (spawner.State.GameOver)
        {
            if (turnText != null) turnText.text = "";
            return;
        }

        bool attackerTurn = spawner.State.AttackerTurn;
        bool playerTurn = (attackerTurn == GameSettings.PlayerIsAttacker);
        IsPlayerTurn = playerTurn;

        // Yaziyi guncelle
        if (turnText != null)
        {
            if (showPlayerPerspective)
                turnText.text = playerTurn ? yourTurnText : opponentTurnText;
            else
                turnText.text = attackerTurn ? attackerTurnText : defenderTurnText;
        }

        // Sira degisimi olayi (tas alti gostergesi icin)
        if (!initialized || playerTurn != lastWasPlayerTurn)
        {
            initialized = true;
            lastWasPlayerTurn = playerTurn;
            OnTurnChanged(playerTurn);
        }
    }

    // Sira degisince cagrilir. Tas alti gostergesi buna gore acilip kapanir.
    void OnTurnChanged(bool playerTurn)
    {
        // TurnHighlighter varsa haberdar et
        var highlighter = GetComponent<TurnHighlighter>();
        if (highlighter == null) highlighter = FindObjectOfType<TurnHighlighter>();
        if (highlighter != null)
            highlighter.SetActiveHighlights(playerTurn);
    }
}