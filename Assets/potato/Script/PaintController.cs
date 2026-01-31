using UnityEngine;

public class PaintController : MonoBehaviour
{
    [Header("Paint Color")]
    [SerializeField]
    private Color _color;

    private PaintStateController _paintStateController;
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Color color, PaintStateController paintStateController)
    {
        _color = color;
        _paintStateController = paintStateController;
        _spriteRenderer.color = color;
    }

    public Color GetColor()
    {
        return _color;
    }

    public void OnClick()
    {
        Debug.Log("OnClick: " + _color);
        _paintStateController.OnClickPaint(this);
    }
}
