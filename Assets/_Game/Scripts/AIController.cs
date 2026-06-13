using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public PieceSpawner spawner;
    public BoardHighlighter highlighter; // son hamle vurgusu

    [Header("AI Settings")]
    public bool aiPlaysAttackers = false; // false = AI savunmaci, sen saldirgansin
    public float thinkDelay = 0.6f;

    [Header("Zorluk")]
    [Tooltip("1 = Kolay (Genc Viking), 2-3 = Orta (Savasci), 4 = Zor (Reis)")]
    public int searchDepth = 3;

    float timer;

    void Update()
    {
        if (spawner == null || spawner.State == null) return;
        if (spawner.State.GameOver) return;

        if (spawner.State.AttackerTurn != aiPlaysAttackers)
        {
            timer = 0f;
            return;
        }

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
            Debug.Log("AI'nin hamlesi kalmadi!");
            return;
        }

        Move bestMove = ChooseBestMove(moves);

        var captured = spawner.State.ApplyMove(
            bestMove.FromRow, bestMove.FromCol, bestMove.ToRow, bestMove.ToCol);

        Transform piece = FindPieceAt(bestMove.FromRow, bestMove.FromCol);
        if (piece != null)
        {
            float offset = (spawner.boardSize - 1) / 2f;
            Vector3 target = new Vector3(bestMove.ToCol - offset, piece.position.y, bestMove.ToRow - offset);
            piece.position = target;
        }

        foreach (var (r, c) in captured)
        {
            Transform victim = FindPieceAt(r, c);
            if (victim != null) Destroy(victim.gameObject);
        }

        // Son hamleyi vurgula (oyuncu AI'in ne oynadigini gorsun)
        if (highlighter != null)
            highlighter.HighlightMove(bestMove.FromRow, bestMove.FromCol, bestMove.ToRow, bestMove.ToCol);

        if (spawner.State.GameOver)
            Debug.Log(spawner.State.AttackerWon ? "SALDIRGANLAR KAZANDI!" : "SAVUNMACILAR KAZANDI!");
    }

    Move ChooseBestMove(List<Move> moves)
    {
        bool maximizing = aiPlaysAttackers;
        Move bestMove = moves[0];
        int bestScore = maximizing ? int.MinValue : int.MaxValue;

        foreach (Move move in moves)
        {
            GameState sim = spawner.State.Clone();
            sim.ApplyMove(move.FromRow, move.FromCol, move.ToRow, move.ToCol);

            int score = Minimax(sim, searchDepth - 1, int.MinValue, int.MaxValue, !maximizing);

            if (maximizing && score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
            else if (!maximizing && score < bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    int Minimax(GameState state, int depth, int alpha, int beta, bool maximizing)
    {
        if (depth == 0 || state.GameOver)
            return state.Evaluate();

        List<Move> moves = state.GetAllLegalMoves();
        if (moves.Count == 0)
            return state.Evaluate();

        if (maximizing)
        {
            int maxScore = int.MinValue;
            foreach (Move move in moves)
            {
                GameState sim = state.Clone();
                sim.ApplyMove(move.FromRow, move.FromCol, move.ToRow, move.ToCol);

                int score = Minimax(sim, depth - 1, alpha, beta, false);
                maxScore = Mathf.Max(maxScore, score);

                alpha = Mathf.Max(alpha, score);
                if (beta <= alpha) break;
            }
            return maxScore;
        }
        else
        {
            int minScore = int.MaxValue;
            foreach (Move move in moves)
            {
                GameState sim = state.Clone();
                sim.ApplyMove(move.FromRow, move.FromCol, move.ToRow, move.ToCol);

                int score = Minimax(sim, depth - 1, alpha, beta, true);
                minScore = Mathf.Min(minScore, score);

                beta = Mathf.Min(beta, score);
                if (beta <= alpha) break;
            }
            return minScore;
        }
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