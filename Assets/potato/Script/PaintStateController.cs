using UnityEngine;

public class PaintStateController : StateController
{
    [Header("Settings")]
    [SerializeField]
    private float _paintSpacing = 2f;

    [Header("Paint Prefab")]
    [SerializeField]
    private GameObject _paintPrefab;

    [Header("Paint Holder")]
    [SerializeField]
    private Transform _paintHolder;

    [Header("Paint")]
    [SerializeField]
    private Color _currentPaintColor;

    public override void OnEnterState()
    {
        Init();
    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }

    private void Init()
    {
        // 마스크 데이터 가져오기
        MaskData maskData = MaskCreateManager.instance.GetCurrentMaskData();

        // 페인트 색상 가져오기
        Color[] colorPalette = maskData.GetColorPalette();
        int count = colorPalette.Length;

        for (int i = 0; i < count; i++)
        {
            GameObject paint = Instantiate(_paintPrefab, _paintHolder);
            paint.GetComponent<PaintController>().Init(colorPalette[i], this);

            // 세로 배치, 간격 2, 가운데 정렬 (위에서 아래로 0, 1, 2...)
            float y = (count - 1) * _paintSpacing * 0.5f - i * _paintSpacing;
            paint.transform.localPosition = new Vector3(0f, y, 0f);
        }
    }

    public void OnClickPaint(PaintController paintController)
    {
        _currentPaintColor = paintController.GetColor();
        MaskController maskController = MaskCreateManager.instance.GetCurrentMaskController();
        maskController.SetPaintColor(_currentPaintColor);
    }
}
