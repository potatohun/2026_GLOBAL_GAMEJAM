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

    public void SetBrushSize(bool isBigBrush)
    {
        if(isBigBrush) {
            _eraseMaskController.radiusUV = 0.04f;
        } else {
            _eraseMaskController.radiusUV = 0.01f;
        }
    }
}
