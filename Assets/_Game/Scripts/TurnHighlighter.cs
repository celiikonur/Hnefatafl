using System.Collections.Generic;
using UnityEngine;

// Oyuncunun sirasi geldiginde, oyuncuya ait TUM taslarin altinda
// gosterge (atan disk) belirir. Sira rakibe gecince kapanir.
//
// Diskler dogrudan dunya konumuna konur (parent'in pozisyon/scale'i
// diskleri etkilemez) -> merkeze yigilma sorunu olmaz.
public class TurnHighlighter : MonoBehaviour
{
    [Header("Referanslar")]
    public PieceSpawner spawner;

    [Tooltip("Tas altina konacak gosterge prefab'i (atan disk)")]
    public GameObject indicatorPrefab;

    [Tooltip("Gosterge tas zemininden ne kadar yukarida (Y)")]
    public float yOffset = 0.05f;

    readonly List<GameObject> activeIndicators = new List<GameObject>();

    // Diskleri toplayacak, dunya merkezli (0,0,0) bir kap. Boylece
    // bu script hangi objede olursa olsun diskler dogru yerde kalir.
    Transform holder;

    void Awake()
    {
        var holderObj = new GameObject("TurnIndicators");
        holder = holderObj.transform;
        holder.position = Vector3.zero;
        holder.rotation = Quaternion.identity;
        holder.localScale = Vector3.one;
    }

    public void SetActiveHighlights(bool playerTurn)
    {
        ClearIndicators();

        if (!playerTurn) return;
        if (spawner == null || spawner.State == null) return;
        if (spawner.State.GameOver) return;
        if (indicatorPrefab == null) return;

        bool playerIsAttacker = GameSettings.PlayerIsAttacker;

        int size = spawner.boardSize;
        float offset = (size - 1) / 2f;

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                PieceType p = spawner.State.GetPiece(row, col);
                if (p == PieceType.None) continue;

                bool pieceIsPlayers;
                if (playerIsAttacker)
                    pieceIsPlayers = (p == PieceType.Attacker);
                else
                    pieceIsPlayers = (p == PieceType.Defender || p == PieceType.King);

                if (!pieceIsPlayers) continue;

                // Dunya konumu: tahtanin uzerindeki dogru kare
                Vector3 pos = new Vector3(col - offset, yOffset, row - offset);

                // Parent olarak holder (0,0,0) kullaniyoruz; instantiate SONRASI
                // dunya pozisyonunu net set ediyoruz ki parent etkisi olmasin.
                GameObject ind = Instantiate(indicatorPrefab, pos, Quaternion.identity, holder);
                ind.transform.position = pos; // garanti: dunya konumu
                activeIndicators.Add(ind);
            }
        }
    }

    void ClearIndicators()
    {
        for (int i = 0; i < activeIndicators.Count; i++)
            if (activeIndicators[i] != null) Destroy(activeIndicators[i]);
        activeIndicators.Clear();
    }
}