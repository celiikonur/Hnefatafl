using UnityEngine;

// Kamera iki aci arasinda yumusak gecis yapar:
//  - Perspektif (normal): Viking'in goruldugu atmosferik aci
//  - Tepeden (top-down): tahtaya dik kusbakisi, net okuma
// Sag ustteki butona baglanir (ToggleView).
public class CameraController : MonoBehaviour
{
    [Header("Perspektif (normal) aci")]
    public Vector3 perspectivePosition = new Vector3(0f, 12f, -8f);
    public Vector3 perspectiveRotation = new Vector3(60f, 0f, 0f);

    [Header("Tepeden (kusbakisi) aci")]
    public Vector3 topPosition = new Vector3(0f, 16f, 0f);
    public Vector3 topRotation = new Vector3(90f, 0f, 0f);

    [Header("Gecis")]
    [Tooltip("Gecis hizi (buyuk = hizli)")]
    public float transitionSpeed = 4f;

    bool isTopView;
    Vector3 targetPos;
    Quaternion targetRot;

    void Start()
    {
        // Baslangic: perspektif
        targetPos = perspectivePosition;
        targetRot = Quaternion.Euler(perspectiveRotation);
        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    void Update()
    {
        // Hedefe yumusak gecis
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * transitionSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * transitionSpeed);
    }

    // Sag ustteki kamera butonu buna baglanir
    public void ToggleView()
    {
        isTopView = !isTopView;

        if (isTopView)
        {
            targetPos = topPosition;
            targetRot = Quaternion.Euler(topRotation);
        }
        else
        {
            targetPos = perspectivePosition;
            targetRot = Quaternion.Euler(perspectiveRotation);
        }
    }
}