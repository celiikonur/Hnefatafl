using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionController : MonoBehaviour
{
    public Material highlightMaterial;
    public PieceSpawner spawner;
    public BoardHighlighter highlighter;

    Transform selectedPiece;
    Material originalMaterial;
    int selectedRow = -1, selectedCol = -1;

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform target = hit.transform;

            if (IsPiece(target))
            {
                Select(target);
            }
            else if (IsTile(target) && selectedPiece != null)
            {
                TryMoveTo(target);
            }
            else
            {
                Deselect();
            }
        }
        else
        {
            Deselect();
        }
    }

    bool IsPiece(Transform t)
    {
        return t.name.StartsWith("Attacker")
            || t.name.StartsWith("Defender")
            || t.name == "King";
    }

    bool IsTile(Transform t)
    {
        return t.name.StartsWith("Tile_");
    }

    (int row, int col) ParseCoords(string objectName)
    {
        string[] parts = objectName.Split('_');
        return (int.Parse(parts[parts.Length - 2]), int.Parse(parts[parts.Length - 1]));
    }

    (int row, int col) WorldToGrid(Vector3 pos)
    {
        float offset = (spawner.boardSize - 1) / 2f;
        return (Mathf.RoundToInt(pos.z + offset), Mathf.RoundToInt(pos.x + offset));
    }

    void Select(Transform piece)
    {
        if (spawner.State.GameOver) return;

        (int row, int col) = WorldToGrid(piece.position);
        PieceType type = spawner.State.GetPiece(row, col);

        bool isAttacker = type == PieceType.Attacker;
        if (isAttacker != spawner.State.AttackerTurn) return;

        Deselect();

        selectedPiece = piece;
        selectedRow = row;
        selectedCol = col;

        Renderer rend = piece.GetComponent<Renderer>();
        originalMaterial = rend.material;
        rend.material = highlightMaterial;

        // Gecerli hamleleri goster
        if (highlighter != null)
        {
            var targets = spawner.State.GetLegalMovesFrom(row, col);
            highlighter.ShowMoveOptions(targets);
        }
    }

    void TryMoveTo(Transform tile)
    {
        (int toRow, int toCol) = ParseCoords(tile.name);

        if (!spawner.State.IsLegalMove(selectedRow, selectedCol, toRow, toCol))
            return;

        int fromRow = selectedRow, fromCol = selectedCol;

        var captured = spawner.State.ApplyMove(selectedRow, selectedCol, toRow, toCol);

        Vector3 target = tile.position;
        target.y = selectedPiece.position.y;
        selectedPiece.position = target;

        foreach (var (r, c) in captured)
            RemovePieceAt(r, c);

        // Hamle gostergelerini temizle, son hamleyi vurgula
        if (highlighter != null)
        {
            highlighter.ClearMoveOptions();
            highlighter.HighlightMove(fromRow, fromCol, toRow, toCol);
        }

        if (spawner.State.GameOver)
            Debug.Log(spawner.State.AttackerWon ? "SALDIRGANLAR KAZANDI!" : "SAVUNMACILAR KAZANDI!");

        Deselect();
    }

    void RemovePieceAt(int row, int col)
    {
        foreach (Transform piece in spawner.transform)
        {
            (int pr, int pc) = WorldToGrid(piece.position);
            if (pr == row && pc == col)
            {
                Destroy(piece.gameObject);
                return;
            }
        }
    }

    void Deselect()
    {
        if (selectedPiece == null) return;

        selectedPiece.GetComponent<Renderer>().material = originalMaterial;
        selectedPiece = null;
        originalMaterial = null;
        selectedRow = -1;
        selectedCol = -1;

        // Secim kalkinca hamle gostergelerini de temizle
        if (highlighter != null)
            highlighter.ClearMoveOptions();
    }
}