using UnityEngine;
using DG.Tweening;

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

    [Header("Brush")]
    [SerializeField] private bool _isBigBrush = false;
    public GameObject _bigBrush;
    public GameObject _smallBrush;
    public override void OnEnterState()
    {
        this.transform.DOMoveY(-11f, 1f).SetEase(Ease.InOutExpo);

        Init();
    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();
    }

    public override void OnExitState()
    {
        this.transform.DOMoveY(-20f, 1f).SetEase(Ease.InOutExpo);
        base.OnExitState();

        CursorManager.instance.EquipTool(CursorType.Hand);
    }

    private void Init()
    {
        // 초기화
        for (int i = 0; i < _paintHolder.childCount; i++)
        {
            Destroy(_paintHolder.GetChild(i).gameObject);
        }

        // 페인트 색상 초기화
        _currentPaintColor = new Color(0f, 0f, 0f, 0f);
        MaskController maskController = MaskCreateManager.instance.GetCurrentMaskController();
        maskController.SetPaintColor(_currentPaintColor);

        // 마스크 데이터 가져오기
        MaskData maskData = MaskCreateManager.instance.GetCurrentMaskData();

        // 페인트 색상 가져오기
        Color[] colorPalette = maskData.GetColorPalette();
        int count = colorPalette.Length;

        // 브러시 크기 설정
        _bigBrush.SetActive(false);
        _smallBrush.SetActive(true);
        CursorManager.instance.EquipTool(CursorType.BigBrush);
        _isBigBrush = true;

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

        CursorManager.instance.SetColor(_currentPaintColor);
    }

    public void OnClickBrush()
    {
        if(_isBigBrush) {
            _bigBrush.SetActive(true);
            _smallBrush.SetActive(false);
            CursorManager.instance.EquipTool(CursorType.Brush);
            _isBigBrush = false;
            MaskCreateManager.instance.GetCurrentMaskController().SetBrushSize(_isBigBrush);
        } else {
            _bigBrush.SetActive(false);
            _smallBrush.SetActive(true);
            CursorManager.instance.EquipTool(CursorType.BigBrush);
            _isBigBrush = true;
            MaskCreateManager.instance.GetCurrentMaskController().SetBrushSize(_isBigBrush);
        }
    }
}
