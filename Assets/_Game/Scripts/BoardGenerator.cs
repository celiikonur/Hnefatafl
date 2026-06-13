using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 11;
    public GameObject tilePrefab;
    public Material lightMaterial;
    public Material darkMaterial;
    public Material specialMaterial;

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        float offset = (boardSize - 1) / 2f;

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Vector3 position = new Vector3(col - offset, 0, row - offset);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile_{row}_{col}";

                Renderer rend = tile.GetComponent<Renderer>();

if (IsSpecialTile(row, col))
{
    rend.material = specialMaterial;
}
else
{
    bool isLight = (row + col) % 2 == 0;
    rend.material = isLight ? lightMaterial : darkMaterial;
}
            }
        }
    }
    bool IsSpecialTile(int row, int col)
{
    int last = boardSize - 1;
    int center = boardSize / 2;

    bool isCorner = (row == 0 || row == last) && (col == 0 || col == last);
    bool isThrone = (row == center && col == center);

    return isCorner || isThrone;
}
}