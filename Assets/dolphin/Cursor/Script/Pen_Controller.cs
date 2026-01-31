using UnityEngine;
using UnityEngine.InputSystem;

public class Pen_Controller : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform particleRect;
    [SerializeField] private ParticleSystem ps;

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetPosition(mousePos);
            ps.Play();
        }

        if (Mouse.current.leftButton.isPressed)
        {
            SetPosition(mousePos);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ps.Stop();
        }
    }

    void SetPosition(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null, // Screen Space - Overlay
            out Vector2 localPos
        );

        particleRect.localPosition = localPos;
    }
}
