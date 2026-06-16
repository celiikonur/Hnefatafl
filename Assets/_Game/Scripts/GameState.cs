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
    public bool IsDraw { get; private set; }

    bool trackRepetition = true;

    static readonly (int dr, int dc)[] Directions = { (1, 0), (-1, 0), (0, 1), (0, -1) };

    Dictionary<string, int> positionHistory;

    public GameState(int size, string[] layout)
    {
        Size = size;
        board = new PieceType[size, size];
        positionHistory = new Dictionary<string, int>();

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

        RecordPosition();
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
        copy.IsDraw = IsDraw;
        copy.trackRepetition = false;
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

    // === MOVE ORDERING (AI hizlandirma) ===
    // Hamleleri "muhtemelen iyi" olanlar one gelecek sekilde siralar.
    // Alpha-beta budamasi boylece cok daha erken devreye girer -> ayni
    // sonuc, daha az pozisyon taramasi. Zeka ayni, hiz artar.
    public List<Move> GetOrderedMoves()
    {
        var moves = GetAllLegalMoves();

        // Her hamleye hizli bir "umut puani" ver, buyukten kucuge sirala.
        moves.Sort((a, b) => MoveHeuristic(b).CompareTo(MoveHeuristic(a)));
        return moves;
    }

    // Hamleyi gercekten uygulamadan, kabaca ne kadar umut verici oldugunu tahmin et.
    int MoveHeuristic(Move m)
    {
        int h = 0;
        PieceType piece = board[m.FromRow, m.FromCol];

        // 1) Capture potansiyeli: hedefin yaninda dusman + arkasi bizim/engel mi?
        foreach (var (dr, dc) in Directions)
        {
            int vr = m.ToRow + dr, vc = m.ToCol + dc;
            if (!IsInside(vr, vc)) continue;

            PieceType victim = board[vr, vc];
            if (victim == PieceType.None) continue;

            bool moverIsAttacker = piece == PieceType.Attacker;
            bool victimIsAttacker = victim == PieceType.Attacker;
            if (moverIsAttacker == victimIsAttacker) continue; // dost

            // Kurbanin obur tarafi dusmanca mi (kaba capture tahmini)
            int or_ = vr + dr, oc = vc + dc;
            bool backHostile =
                IsCorner(or_, oc) ||
                (IsInside(or_, oc) && board[or_, oc] != PieceType.None &&
                 (board[or_, oc] == PieceType.Attacker) != victimIsAttacker);

            if (victim == PieceType.King) h += 500;      // krala dokunmak cok degerli
            else if (backHostile) h += 100;              // muhtemel capture
            else h += 10;                                // dusman komsulugu
        }

        // 2) Kral koseye dogru gidiyorsa (savunmaci icin degerli)
        if (piece == PieceType.King)
        {
            int last = Size - 1;
            int distNow = CornerDist(m.FromRow, m.FromCol);
            int distAfter = CornerDist(m.ToRow, m.ToCol);
            if (distAfter < distNow) h += 200;           // koseye yaklasiyor
            if (IsCorner(m.ToRow, m.ToCol)) h += 10000;  // direkt kazanc!
        }

        return h;
    }

    int CornerDist(int row, int col)
    {
        int last = Size - 1;
        return Mathf.Min(
            Mathf.Max(row, col),
            Mathf.Max(last - row, col),
            Mathf.Max(row, last - col),
            Mathf.Max(last - row, last - col)
        );
    }

    public List<(int row, int col)> GetLegalMovesFrom(int row, int col)
    {
        var targets = new List<(int, int)>();
        if (GameOver) return targets;
        if (!IsInside(row, col)) return targets;

        PieceType piece = board[row, col];
        if (piece == PieceType.None) return targets;

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

        if (trackRepetition && !GameOver)
            CheckRepetition();

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
                if (IsKingCaptured(vr, vc))
                    captured.Add((vr, vc));
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

    bool IsKingCaptured(int kingRow, int kingCol)
    {
        foreach (var (dr, dc) in Directions)
        {
            int nr = kingRow + dr, nc = kingCol + dc;
            if (!IsInside(nr, nc)) continue;
            bool hostile = board[nr, nc] == PieceType.Attacker || IsThrone(nr, nc);
            if (!hostile) return false;
        }
        return true;
    }

    void RecordPosition()
    {
        if (positionHistory == null) return;
        string key = PositionKey();
        if (positionHistory.ContainsKey(key))
            positionHistory[key]++;
        else
            positionHistory[key] = 1;
    }

    void CheckRepetition()
    {
        if (positionHistory == null) return;
        RecordPosition();
        string key = PositionKey();
        if (positionHistory[key] >= 3)
        {
            GameOver = true;
            IsDraw = true;
        }
    }

    string PositionKey()
    {
        var sb = new System.Text.StringBuilder(Size * Size + 1);
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                sb.Append((int)board[r, c]);
        sb.Append(AttackerTurn ? 'A' : 'D');
        return sb.ToString();
    }

    public int Evaluate()
    {
        if (GameOver)
        {
            if (IsDraw) return 0;
            return AttackerWon ? 100000 : -100000;
        }

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
            int distToCorner = CornerDist(kingRow, kingCol);
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