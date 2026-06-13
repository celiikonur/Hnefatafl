using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public PieceSpawner spawner;

    [Header("AI Settings")]
    public bool aiPlaysAttackers = false; // false = AI savunmaci (sen saldirgansin ve ilk hamle sende)
    public float thinkDelay = 0.6f; // insansi bir bekleme, aninda oynamasin

    float timer;

    void Update()
    {
        if (spawner == null || spawner.State == null) return;
        if (spawner.State.GameOver) return;

        // Sira AI'da mi?
        if (spawner.State.AttackerTurn != aiPlaysAttackers)
        {
            timer = 0f;
            return;
        }

        // Dusunme suresi dolmadan oynama
        timer += Time.deltaTime;
        if (timer < thinkDelay) return;
        timer = 0f;

        PlayTurn();
    }

    void PlayTurn()
    {
        List<Move> moves = spawner.State.GetAllLegalMoves();

        if (moves.Count == 0)
        {
            // Hamlesi kalmayan taraf kaybeder (klasik tafl kurali)
            Debug.Log("AI'nin hamlesi kalmadi!");
            return;
        }

        // v1: rastgele sec. (v2'de burasi minimax olacak, gerisi ayni kalacak)
        Move move = moves[Random.Range(0, moves.Count)];

        var captured = spawner.State.ApplyMove(move.FromRow, move.FromCol, move.ToRow, move.ToCol);

        // === Gorseli guncelle ===
        Transform piece = FindPieceAt(move.FromRow, move.FromCol);
        if (piece != null)
        {
            float offset = (spawner.boardSize - 1) / 2f;
            Vector3 target = new Vector3(move.ToCol - offset, piece.position.y, move.ToRow - offset);
            piece.position = target;
        }

        foreach (var (r, c) in captured)
        {
            Transform victim = FindPieceAt(r, c);
            if (victim != null) Destroy(victim.gameObject);
        }

        if (spawner.State.GameOver)
            Debug.Log(spawner.State.AttackerWon ? "SALDIRGANLAR KAZANDI!" : "SAVUNMACILAR KAZANDI!");
    }

    Transform FindPieceAt(int row, int col)
    {
        float offset = (spawner.boardSize - 1) / 2f;

        foreach (Transform piece in spawner.transform)
        {
            int pr = Mathf.RoundToInt(piece.position.z + offset);
            int pc = Mathf.RoundToInt(piece.position.x + offset);
            if (pr == row && pc == col) return piece;
        }
        return null;
    }
}