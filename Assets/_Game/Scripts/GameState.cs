using System.Collections.Generic;
using UnityEngine;

public enum PieceType { None, Attacker, Defender, King }

public struct Move
{
    public int FromRow, FromCol, ToRow, ToCol;

    public Move(int fromRow, int fromCol, int toRow, int toCol)
    {
        FromRow = fromRow; FromCol = fromCol;
        ToRow = toRow; ToCol = toCol;
    }
}

public class GameState
{
    public int Size { get; private set; }
    PieceType[,] board;

    public bool AttackerTurn { get; private set; } = true;
    public bool GameOver { get; private set; }
    public bool AttackerWon { get; private set; }

    static readonly (int dr, int dc)[] Directions = { (1, 0), (-1, 0), (0, 1), (0, -1) };

    public GameState(int size, string[] layout)
    {
        Size = size;
        board = new PieceType[size, size];

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                board[row, col] = layout[row][col] switch
                {
                    'A' => PieceType.Attacker,
                    'D' => PieceType.Defender,
                    'K' => PieceType.King,
                    _ => PieceType.None
                };
            }
        }
    }

    GameState(int size)
    {
        Size = size;
        board = new PieceType[size, size];
    }

    public GameState Clone()
    {
        var copy = new GameState(Size);
        System.Array.Copy(board, copy.board, board.Length);
        copy.AttackerTurn = AttackerTurn;
        copy.GameOver = GameOver;
        copy.AttackerWon = AttackerWon;
        return copy;
    }

    public PieceType GetPiece(int row, int col) => board[row, col];

    public bool IsInside(int row, int col) =>
        row >= 0 && row < Size && col >= 0 && col < Size;

    public bool IsCorner(int row, int col)
    {
        int last = Size - 1;
        return (row == 0 || row == last) && (col == 0 || col == last);
    }

    public bool IsThrone(int row, int col)
    {
        int c = Size / 2;
        return row == c && col == c;
    }

    public bool IsLegalMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (!IsInside(fromRow, fromCol) || !IsInside(toRow, toCol)) return false;
        if (GameOver) return false;

        PieceType piece = board[fromRow, fromCol];
        if (piece == PieceType.None) return false;

        bool isAttackerPiece = piece == PieceType.Attacker;
        if (isAttackerPiece != AttackerTurn) return false;

        if (board[toRow, toCol] != PieceType.None) return false;

        if (fromRow != toRow && fromCol != toCol) return false;
        if (fromRow == toRow && fromCol == toCol) return false;

        int stepRow = System.Math.Sign(toRow - fromRow);
        int stepCol = System.Math.Sign(toCol - fromCol);
        int r = fromRow + stepRow, c = fromCol + stepCol;
        while (r != toRow || c != toCol)
        {
            if (board[r, c] != PieceType.None) return false;
            r += stepRow;
            c += stepCol;
        }

        bool isRestricted = IsCorner(toRow, toCol) || IsThrone(toRow, toCol);
        if (isRestricted && piece != PieceType.King) return false;

        return true;
    }

    public List<Move> GetAllLegalMoves()
    {
        var moves = new List<Move>();
        if (GameOver) return moves;

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                PieceType piece = board[row, col];
                if (piece == PieceType.None) continue;

                bool isAttackerPiece = piece == PieceType.Attacker;
                if (isAttackerPiece != AttackerTurn) continue;

                foreach (var (dr, dc) in Directions)
                {
                    int r = row + dr, c = col + dc;
                    while (IsInside(r, c) && board[r, c] == PieceType.None)
                    {
                        bool isRestricted = IsCorner(r, c) || IsThrone(r, c);
                        if (!isRestricted || piece == PieceType.King)
                            moves.Add(new Move(row, col, r, c));

                        r += dr;
                        c += dc;
                    }
                }
            }
        }

        return moves;
    }

    // === TEK TASIN HEDEFLERI (gecerli hamle gostergeleri icin) ===
    // Verilen karedeki tasin gidebilecegi tum kareleri dondurur.
    public List<(int row, int col)> GetLegalMovesFrom(int row, int col)
    {
        var targets = new List<(int, int)>();
        if (GameOver) return targets;
        if (!IsInside(row, col)) return targets;

        PieceType piece = board[row, col];
        if (piece == PieceType.None) return targets;

        // Sirasi degilse gosterme
        bool isAttackerPiece = piece == PieceType.Attacker;
        if (isAttackerPiece != AttackerTurn) return targets;

        foreach (var (dr, dc) in Directions)
        {
            int r = row + dr, c = col + dc;
            while (IsInside(r, c) && board[r, c] == PieceType.None)
            {
                bool isRestricted = IsCorner(r, c) || IsThrone(r, c);
                if (!isRestricted || piece == PieceType.King)
                    targets.Add((r, c));

                r += dr;
                c += dc;
            }
        }

        return targets;
    }

    public List<(int row, int col)> ApplyMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        board[toRow, toCol] = board[fromRow, fromCol];
        board[fromRow, fromCol] = PieceType.None;

        var captured = CheckCaptures(toRow, toCol);

        foreach (var (r, c) in captured)
        {
            if (board[r, c] == PieceType.King)
            {
                GameOver = true;
                AttackerWon = true;
            }
            board[r, c] = PieceType.None;
        }

        if (board[toRow, toCol] == PieceType.King && IsCorner(toRow, toCol))
        {
            GameOver = true;
            AttackerWon = false;
        }

        AttackerTurn = !AttackerTurn;
        return captured;
    }

    bool IsHostileTo(int row, int col, PieceType victim)
    {
        if (!IsInside(row, col)) return false;

        PieceType occupant = board[row, col];

        if (IsCorner(row, col)) return true;
        if (IsThrone(row, col) && occupant == PieceType.None) return true;

        bool victimIsAttacker = victim == PieceType.Attacker;
        bool occupantIsAttacker = occupant == PieceType.Attacker;
        return occupant != PieceType.None && victimIsAttacker != occupantIsAttacker;
    }

    List<(int, int)> CheckCaptures(int moverRow, int moverCol)
    {
        var captured = new List<(int, int)>();
        PieceType mover = board[moverRow, moverCol];

        foreach (var (dr, dc) in Directions)
        {
            int vr = moverRow + dr, vc = moverCol + dc;

            if (!IsInside(vr, vc)) continue;

            PieceType victim = board[vr, vc];
            if (victim == PieceType.None) continue;

            bool moverIsAttacker = mover == PieceType.Attacker;
            bool victimIsAttacker = victim == PieceType.Attacker;
            if (moverIsAttacker == victimIsAttacker) continue;

            if (victim == PieceType.King)
            {
                bool surrounded = true;
                foreach (var (kr, kc) in Directions)
                {
                    int nr = vr + kr, nc = vc + kc;
                    if (!IsInside(nr, nc)) { surrounded = false; break; }
                    if (board[nr, nc] != PieceType.Attacker && !IsThrone(nr, nc))
                    {
                        surrounded = false;
                        break;
                    }
                }
                if (surrounded) captured.Add((vr, vc));
            }
            else
            {
                int or_ = vr + dr, oc = vc + dc;
                if (IsHostileTo(or_, oc, victim))
                    captured.Add((vr, vc));
            }
        }

        return captured;
    }

    public int Evaluate()
    {
        if (GameOver)
            return AttackerWon ? 100000 : -100000;

        int score = 0;
        int kingRow = -1, kingCol = -1;
        int attackerCount = 0, defenderCount = 0;

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                switch (board[r, c])
                {
                    case PieceType.Attacker: attackerCount++; break;
                    case PieceType.Defender: defenderCount++; break;
                    case PieceType.King: kingRow = r; kingCol = c; break;
                }
            }
        }

        score += attackerCount * 10;
        score -= defenderCount * 15;

        if (kingRow >= 0)
        {
            int last = Size - 1;
            int distToCorner = Mathf.Min(
                Mathf.Max(kingRow, kingCol),
                Mathf.Max(last - kingRow, kingCol),
                Mathf.Max(kingRow, last - kingCol),
                Mathf.Max(last - kingRow, last - kingCol)
            );
            score -= (Size - distToCorner) * 8;

            int pressure = 0;
            foreach (var (dr, dc) in Directions)
            {
                int nr = kingRow + dr, nc = kingCol + dc;
                if (IsInside(nr, nc) && board[nr, nc] == PieceType.Attacker)
                    pressure++;
            }
            score += pressure * 12;
        }

        return score;
    }
}