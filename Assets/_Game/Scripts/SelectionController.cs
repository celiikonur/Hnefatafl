using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionController : MonoBehaviour
{
    public Material highlightMaterial;
    public PieceSpawner spawner;
    public BoardHighlighter highlighter;
    public PieceAnimator animator;

    Transform selectedPiece;
    Renderer selectedRenderer;
    Material originalMaterial;
    int selectedRow = -1, selectedCol = -1;

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (animator != null && animator.IsAnimating) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform target = hit.transform;

            // Tiklanan sey bir tasin child'i olabilir; ust parent'a cik
            Transform pieceRoot = FindPieceRoot(target);

            if (pieceRoot != null)
            {
                Select(pieceRoot);
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

    // Tiklanan transform veya parent'lari bir tas mi? Tas root'unu dondur.
    Transform FindPieceRoot(Transform t)
    {
        Transform current = t;
        while (current != null)
        {
            if (IsPieceName(current.name))
                return current;
            current = current.parent;
        }
        return null;
    }

    bool IsPieceName(string n)
    {
        return n.StartsWith("Attacker")
            || n.StartsWith("Defender")
            || n == "King";
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

        selectedRenderer = piece.GetComponentInChildren<Renderer>();
        if (selectedRenderer != null)
        {
            originalMaterial = selectedRenderer.material;
            selectedRenderer.material = highlightMaterial;
        }

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

        Transform movingPiece = selectedPiece;

        // Secim materyalini geri yukle
        if (selectedRenderer != null && originalMaterial != null)
            selectedRenderer.material = originalMaterial;

        Vector3 target = tile.position;
        target.y = movingPiece.position.y;

        if (highlighter != null)
        {
            highlighter.ClearMoveOptions();
            highlighter.HighlightMove(fromRow, fromCol, toRow, toCol);
        }

        if (animator != null)
        {
            animator.MovePiece(movingPiece, target, () =>
            {
                FinishMove(captured);
            });
        }
        else
        {
            movingPiece.position = target;
            FinishMove(captured);
        }

        selectedPiece = null;
        selectedRenderer = null;
        originalMaterial = null;
        selectedRow = -1;
        selectedCol = -1;
    }

    void FinishMove(System.Collections.Generic.List<(int row, int col)> captured)
    {
        foreach (var (r, c) in captured)
            RemovePieceAt(r, c);

        if (spawner.State.GameOver)
            Debug.Log(spawner.State.AttackerWon ? "SALDIRGANLAR KAZANDI!" : "SAVUNMACILAR KAZANDI!");
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

        if (selectedRenderer != null && originalMaterial != null)
            selectedRenderer.material = originalMaterial;

        selectedPiece = null;
        selectedRenderer = null;
        originalMaterial = null;
        selectedRow = -1;
        selectedCol = -1;

        if (highlighter != null)
            highlighter.ClearMoveOptions();
    }
}