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

    public bool AttackerTurn { get; private set; } = true; // Hnefatafl'da ilk hamle saldirganin
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

    // === HAMLE GECERLILIGI ===
    public bool IsLegalMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (!IsInside(fromRow, fromCol) || !IsInside(toRow, toCol)) return false;
        if (GameOver) return false;

        PieceType piece = board[fromRow, fromCol];
        if (piece == PieceType.None) return false;

        // Sira kontrolu
        bool isAttackerPiece = piece == PieceType.Attacker;
        if (isAttackerPiece != AttackerTurn) return false;

        // Hedef dolu olamaz
        if (board[toRow, toCol] != PieceType.None) return false;

        // Sadece duz hat (kale gibi)
        if (fromRow != toRow && fromCol != toCol) return false;
        if (fromRow == toRow && fromCol == toCol) return false;

        // Yol uzerinde tas olamaz
        int stepRow = System.Math.Sign(toRow - fromRow);
        int stepCol = System.Math.Sign(toCol - fromCol);
        int r = fromRow + stepRow, c = fromCol + stepCol;
        while (r != toRow || c != toCol)
        {
            if (board[r, c] != PieceType.None) return false;
            r += stepRow;
            c += stepCol;
        }

        // Koselere ve tahta sadece kral girebilir
        bool isRestricted = IsCorner(toRow, toCol) || IsThrone(toRow, toCol);
        if (isRestricted && piece != PieceType.King) return false;

        return true;
    }

    // === TUM YASAL HAMLELER ===
    // Siradaki oyuncunun yapabilecegi her hamleyi listeler (AI bunu kullanacak)
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

                // Tas kale gibi gider: 4 yonde, engele kadar tara
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

    // === HAMLE UYGULAMA ===
    // Donus degeri: yenen taslarin koordinat listesi (gorsel katman bunlari sahneden silecek)
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

        // Kral koseye ulasti mi?
        if (board[toRow, toCol] == PieceType.King && IsCorner(toRow, toCol))
        {
            GameOver = true;
            AttackerWon = false;
        }

        AttackerTurn = !AttackerTurn;
        return captured;
    }

    // === CAPTURE MANTIGI ===
    bool IsHostileTo(int row, int col, PieceType victim)
    {
        // Tahta disi dusmanca degildir
        if (!IsInside(row, col)) return false;

        PieceType occupant = board[row, col];

        // Koseler herkese dusmanca
        if (IsCorner(row, col)) return true;

        // Bos taht herkese dusmanca
        if (IsThrone(row, col) && occupant == PieceType.None) return true;

        // Dusman tasi mi?
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
            int vr = moverRow + dr, vc = moverCol + dc; // kurban adayi

            if (!IsInside(vr, vc)) continue;

            PieceType victim = board[vr, vc];
            if (victim == PieceType.None) continue;

            // Kurban dusman mi?
            bool moverIsAttacker = mover == PieceType.Attacker;
            bool victimIsAttacker = victim == PieceType.Attacker;
            if (moverIsAttacker == victimIsAttacker) continue;

            if (victim == PieceType.King)
            {
                // Kral: 4 tarafi da dusmanca olmali (saldirgan veya taht)
                bool surrounded = true;
                foreach (var (kr, kc) in Directions)
                {
                    int nr = vr + kr, nc = vc + kc;
                    if (!IsInside(nr, nc)) { surrounded = false; break; } // kenarda yakalanmaz
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
                // Normal tas: karsi taraf dusmanca mi? (sandvic)
                int or_ = vr + dr, oc = vc + dc; // kurbanin obur tarafi
                if (IsHostileTo(or_, oc, victim))
                    captured.Add((vr, vc));
            }
        }

        return captured;
    }
}