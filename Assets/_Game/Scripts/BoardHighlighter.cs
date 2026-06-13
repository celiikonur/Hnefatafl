using UnityEngine;

// Son yapilan hamlenin kalkis ve varis karelerini, uzerlerine seffaf
// glow levhalari koyarak vurgular. Karelerin kendi rengine/dokusuna
// dokunmaz; sadece ustlerine isik dusmus gibi gorunur.
public class BoardHighlighter : MonoBehaviour
{
    public Transform board;          // Tile'larin parent'i (Board objesi)
    public GameObject highlightPrefab; // HighlightTile prefab'i (seffaf glow levha)
    public float heightOffset = 0.08f; // Levha karenin ne kadar ustunde dursun

    GameObject fromGlow, toGlow;

    void Awake()
    {
        // Iki levhayi bir kere uret, sonra surekli tasiyip gosterip gizleyecegiz
        fromGlow = Instantiate(highlightPrefab, transform);
        toGlow = Instantiate(highlightPrefab, transform);
        fromGlow.SetActive(false);
        toGlow.SetActive(false);
    }

    // Bir hamleyi vurgula: levhalari ilgili karelerin ustune tasi ve goster
    public void HighlightMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        PlaceGlow(fromGlow, fromRow, fromCol);
        PlaceGlow(toGlow, toRow, toCol);
    }

    // Vurguyu gizle
    public void ClearHighlight()
    {
        if (fromGlow != null) fromGlow.SetActive(false);
        if (toGlow != null) toGlow.SetActive(false);
    }

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
        pos.y += heightOffset; // karenin biraz ustune
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