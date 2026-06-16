using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public PieceSpawner spawner;
    public BoardHighlighter highlighter;
    public PieceAnimator animator;

    [Header("AI Settings")]
    public bool aiPlaysAttackers = false;
    public float thinkDelay = 0.6f;

    [Header("Zorluk")]
    public int searchDepth = 3;
    [Range(0f, 1f)]
    public float mistakeChance = 0.15f; // en iyi yerine rastgele hamle olasiligi

    [Tooltip("Acikken taraf ve zorlugu GameSettings'ten alir.")]
    public bool useSettings = true;

    float timer;
    bool moveInProgress;

    void Start()
    {
        if (useSettings)
        {
            aiPlaysAttackers = !GameSettings.PlayerIsAttacker;
            searchDepth = GameSettings.SearchDepth;
            mistakeChance = GameSettings.MistakeChance;
        }
    }

    void Update()
    {
        if (spawner == null || spawner.State == null) return;
        if (spawner.State.GameOver) return;
        if (moveInProgress) return;

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
        List<Move> moves = spawner.State.GetOrderedMoves();

        if (moves.Count == 0)
        {
            Debug.Log("AI'nin hamlesi kalmadi!");
            return;
        }

        Move chosen;

        // Hata payi: bazen en iyi hamle yerine rastgele yasal hamle oyna (acemi gibi).
        // Zor'da mistakeChance=0 oldugu icin bu blok hic calismaz.
        if (mistakeChance > 0f && Random.value < mistakeChance)
        {
            chosen = moves[Random.Range(0, moves.Count)];
        }
        else
        {
            chosen = ChooseBestMove(moves);
        }

        var captured = spawner.State.ApplyMove(
            chosen.FromRow, chosen.FromCol, chosen.ToRow, chosen.ToCol);

        Transform piece = FindPieceAt(chosen.FromRow, chosen.FromCol);

        if (highlighter != null)
            highlighter.HighlightMove(chosen.FromRow, chosen.FromCol, chosen.ToRow, chosen.ToCol);

        float offset = (spawner.boardSize - 1) / 2f;
        Vector3 target = new Vector3(
            chosen.ToCol - offset,
            piece != null ? piece.position.y : 0.2f,
            chosen.ToRow - offset);

        moveInProgress = true;

        if (animator != null && piece != null)
        {
            animator.MovePiece(piece, target, () =>
            {
                FinishMove(captured);
                moveInProgress = false;
            });
        }
        else
        {
            if (piece != null) piece.position = target;
            FinishMove(captured);
            moveInProgress = false;
        }
    }

    void FinishMove(List<(int row, int col)> captured)
    {
        foreach (var (r, c) in captured)
        {
            Transform victim = FindPieceAt(r, c);
            if (victim != null) Destroy(victim.gameObject);
        }

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
        if (state.GameOver)
        {
            int raw = state.Evaluate();
            if (raw > 0) return raw - (searchDepth - depth);
            if (raw < 0) return raw + (searchDepth - depth);
            return 0;
        }

        if (depth == 0)
            return state.Evaluate();

        List<Move> moves = state.GetOrderedMoves();
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