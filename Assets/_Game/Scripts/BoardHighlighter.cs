using System.Collections.Generic;
using UnityEngine;

// Iki tur vurgu yonetir:
// 1) Son yapilan hamlenin kalkis/varis kareleri (fromGlow, toGlow)
// 2) Secili tasin gidebilecegi kareler (moveGlows havuzu)
// Hepsi ayni seffaf glow levhasini (HighlightTile) kullanir.
public class BoardHighlighter : MonoBehaviour
{
    public Transform board;            // Tile'larin parent'i (Board objesi)
    public GameObject highlightPrefab; // HighlightTile prefab'i
    public float heightOffset = 0.08f;

    // --- Son hamle vurgusu ---
    GameObject fromGlow, toGlow;

    // --- Gecerli hamle gostergeleri (havuz) ---
    readonly List<GameObject> moveGlows = new List<GameObject>();

    void Awake()
    {
        fromGlow = Instantiate(highlightPrefab, transform);
        toGlow = Instantiate(highlightPrefab, transform);
        fromGlow.SetActive(false);
        toGlow.SetActive(false);
    }

    // === SON HAMLE ===
    public void HighlightMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        PlaceGlow(fromGlow, fromRow, fromCol);
        PlaceGlow(toGlow, toRow, toCol);
    }

    public void ClearHighlight()
    {
        if (fromGlow != null) fromGlow.SetActive(false);
        if (toGlow != null) toGlow.SetActive(false);
    }

    // === GECERLI HAMLELER ===
    // Verilen koordinat listesindeki her kareye bir glow koyar.
    public void ShowMoveOptions(List<(int row, int col)> targets)
    {
        ClearMoveOptions();

        foreach (var (r, c) in targets)
        {
            GameObject glow = GetPooledGlow();
            PlaceGlow(glow, r, c);
        }
    }

    public void ClearMoveOptions()
    {
        foreach (var glow in moveGlows)
            glow.SetActive(false);
    }

    // Havuzdan bos bir glow al; yoksa yeni uret (tekrar tekrar kullanilir)
    GameObject GetPooledGlow()
    {
        foreach (var glow in moveGlows)
        {
            if (!glow.activeSelf) return glow;
        }
        GameObject created = Instantiate(highlightPrefab, transform);
        created.SetActive(false);
        moveGlows.Add(created);
        return created;
    }

    // === ORTAK ===
    void PlaceGlow(GameObject glow, int row, int col)
    {
        if (glow == null) return;

        Transform tile = GetTile(row, col);
        if (tile == null)
        {
            glow.SetActive(false);
            return;
        }

        Vector3 pos = tile.position;
        pos.y += heightOffset;
        glow.transform.position = pos;
        glow.SetActive(true);
    }

    Transform GetTile(int row, int col)
    {
        string targetName = $"Tile_{row}_{col}";
        foreach (Transform tile in board)
        {
            if (tile.name == targetName)
                return tile;
        }
        return null;
    }
}