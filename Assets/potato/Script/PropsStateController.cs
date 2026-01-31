using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PropsStateController : StateController
{
    [Header("Settings")]
    [SerializeField] private float _propsSpacing = 2f;
    [Tooltip("_propsHolder 내부에서 무작위 배치할 가로 범위 (로컬 X ± 이 값의 절반)")]
    [SerializeField] private float _propsSpreadWidth = 4f;
    [Tooltip("_propsHolder 내부에서 무작위 배치할 세로 범위 (로컬 Y ± 이 값의 절반)")]
    [SerializeField] private float _propsSpreadHeight = 4f;

    [Header("Props Holder")]
    [SerializeField] private Transform _propsHolder;

    [Header("Props")]
    [SerializeField]
    private List<GameObject> _currentProps;

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
    }

    private void Init()
    {
        // 초기화
        for (int i = 0; i < _propsHolder.childCount; i++)
        {
            Destroy(_propsHolder.GetChild(i).gameObject);
        }

        _currentProps = new List<GameObject>();

        // 마스크 데이터 가져오기
        MaskData maskData = MaskCreateManager.instance.GetCurrentMaskData();

        // 페인트 색상 가져오기
        GameObject[] props = maskData.GetProps();
        int count = props.Length;

        if (count <= 0) return;

        float halfW = _propsSpreadWidth * 0.5f;
        float halfH = _propsSpreadHeight * 0.5f;

        // 넓게 펼치기: 영역을 셀로 나누고, 각 prop을 셀 안의 무작위 위치에 배치
        int cols = Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(count)));
        int rows = Mathf.Max(1, Mathf.CeilToInt((float)count / cols));
        float cellW = _propsSpreadWidth / cols;
        float cellH = _propsSpreadHeight / rows;

        for (int i = 0; i < count; i++)
        {
            GameObject prop = Instantiate(props[i], _propsHolder);

            int col = i % cols;
            int row = i / cols;
            // 해당 셀 안에서 무작위 위치 (한쪽에 몰리지 않게)
            float xMin = -halfW + col * cellW;
            float yMin = -halfH + row * cellH;
            float x = Random.Range(xMin, xMin + cellW);
            float y = Random.Range(yMin, yMin + cellH);
            prop.transform.localPosition = new Vector3(x, y, 0f);
        }
    }
}
