using UnityEngine;

public class MaskController : MonoBehaviour
{
    EraseMaskController _eraseMaskController;

    void Awake()
    {
        _eraseMaskController = GetComponentInChildren<EraseMaskController>();
    }

    public void SetPaintColor(Color color)
    {
        _eraseMaskController.SetPaintColor(color);
    }
}
