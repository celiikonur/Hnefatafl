using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public GameObject piecePrefab;
    public GameObject kingPrefab;
    public Material attackerMaterial;
    public Material defenderMaterial;
    public Material kingMaterial;

    [Tooltip("Calisma aninda GameSettings'ten okunur; burasi sadece varsayilan.")]
    public int boardSize = 11;

    public GameState State { get; private set; }

    static readonly string[] Layout11 = new string[]
    {
        "...AAAAA...",
        ".....A.....",
        "...........",
        "A....D....A",
        "A...DDD...A",
        "AA.DDKDD.AA",
        "A...DDD...A",
        "A....D....A",
        "...........",
        ".....A.....",
        "...AAAAA..."
    };

    static readonly string[] Layout9 = new string[]
    {
        "...AAA...",
        "....A....",
        "....D....",
        "A...D...A",
        "AA.DKD.AA",
        "A...D...A",
        "....D....",
        "....A....",
        "...AAA..."
    };

    void Start()
    {
        boardSize = GameSettings.BoardSize;

        string[] layout = (boardSize == 9) ? Layout9 : Layout11;

        State = new GameState(boardSize, layout);
        SpawnPieces(layout);
    }

    void SpawnPieces(string[] layout)
    {
        float offset = (boardSize - 1) / 2f;

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                char cell = layout[row][col];
                if (cell == '.') continue;

                Vector3 position = new Vector3(col - offset, 0.2f, row - offset);

                GameObject prefab = (cell == 'K') ? kingPrefab : piecePrefab;
                GameObject piece = Instantiate(prefab, position, Quaternion.identity, transform);

                // Renderer ana objede de olabilir, child'da da (model prefablari genelde child'da tutar)
                Renderer rend = piece.GetComponentInChildren<Renderer>();

                switch (cell)
                {
                    case 'A':
                        if (rend != null) rend.material = attackerMaterial;
                        piece.name = $"Attacker_{row}_{col}";
                        break;
                    case 'D':
                        if (rend != null) rend.material = defenderMaterial;
                        piece.name = $"Defender_{row}_{col}";
                        break;
                    case 'K':
                        if (rend != null) rend.material = kingMaterial;
                        piece.name = "King";
                        break;
                }
            }
        }
    }
}