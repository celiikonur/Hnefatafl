using System.Collections;
using UnityEngine;

// Bir tasi mevcut konumundan hedef konuma yumusakca kaydirir.
// Sadece gorsel; oyun mantigi (GameState) hamleyi zaten aninda uygulamis olur.
public class PieceAnimator : MonoBehaviour
{
    [Tooltip("Kayma suresi (saniye)")]
    public float moveDuration = 0.2f;

    // Su an bir animasyon oynuyor mu? (controller'lar bunu kontrol eder)
    public bool IsAnimating { get; private set; }

    // Tasi hedefe kaydir. Bittiginde onComplete callback'i cagrilir.
    public void MovePiece(Transform piece, Vector3 targetPosition, System.Action onComplete = null)
    {
        if (piece == null)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(MoveRoutine(piece, targetPosition, onComplete));
    }

    IEnumerator MoveRoutine(Transform piece, Vector3 target, System.Action onComplete)
    {
        IsAnimating = true;

        Vector3 start = piece.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            // Taso yok edilmis olabilir (capture vs.) - guvenlik
            if (piece == null)
            {
                IsAnimating = false;
                onComplete?.Invoke();
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // SmoothStep ile hafif yumusama (basta ve sonda yavas)
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            piece.position = Vector3.Lerp(start, target, smooth);

            yield return null; // bir sonraki frame'e kadar bekle
        }

        if (piece != null)
            piece.position = target; // tam hedefe otur

        IsAnimating = false;
        onComplete?.Invoke();
    }
}