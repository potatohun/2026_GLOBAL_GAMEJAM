using UnityEngine;
using UnityEngine.InputSystem;

public class Knife_Controller: MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform particleRect;
    [SerializeField] public ParticleSystem ps;

    [SerializeField] private float currentTime = 0f;
    [SerializeField] private float delayTime = 0.5f;

    private void OnEnable()
    {
        currentTime = 0f;
    }
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
            if (currentTime > delayTime)
            {
                AudioManager.instance.PlayKnifeSound();
                currentTime = 0f;
            }
            else
            {
                currentTime += Time.deltaTime;
            }
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
