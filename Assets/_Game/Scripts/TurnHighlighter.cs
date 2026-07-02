using System.Collections.Generic;
using UnityEngine;

// Oyuncunun sirasi geldiginde, oynayabilecegi taslarin altinda
// bir gosterge (parlayan/donen halka) belirir. Sira rakibe gecince kapanir.
//
// indicatorPrefab: taslarin altina konacak gosterge (donen halka, glow disk vs.)
// Prefab'in kendisi Animator/rotation ile donebilir (gorsel is).
public class TurnHighlighter : MonoBehaviour
{
    [Header("Referanslar")]
    public PieceSpawner spawner;

    [Tooltip("Tas altina konacak gosterge prefab'i (donen halka / glow)")]
    public GameObject indicatorPrefab;

    [Tooltip("Gosterge tas zemininden ne kadar yukarida (Y)")]
    public float yOffset = 0.05f;

    readonly List<GameObject> activeIndicators = new List<GameObject>();

    // TurnIndicator sira degisince bunu cagirir
    public void SetActiveHighlights(bool playerTurn)
    {
        ClearIndicators();

        if (!playerTurn) return;                 // rakip sirasi: gosterge yok
        if (spawner == null || spawner.State == null) return;
        if (spawner.State.GameOver) return;
        if (indicatorPrefab == null) return;

        // Oyuncunun oynayabilecegi taslarin altina gosterge koy
        var moves = spawner.State.GetAllLegalMoves();

        // Ayni tastan birden fazla hamle olabilir; benzersiz kaynak kareler
        var seen = new HashSet<(int, int)>();
        float offset = (spawner.boardSize - 1) / 2f;

        foreach (var m in moves)
        {
            var key = (m.FromRow, m.FromCol);
            if (seen.Contains(key)) continue;
            seen.Add(key);

            Vector3 pos = new Vector3(m.FromCol - offset, yOffset, m.FromRow - offset);
            GameObject ind = Instantiate(indicatorPrefab, pos, Quaternion.identity, transform);
            activeIndicators.Add(ind);
        }
    }

    void ClearIndicators()
    {
        foreach (var ind in activeIndicators)
            if (ind != null) Destroy(ind);
        activeIndicators.Clear();
    }
}