using UnityEngine;
using System.Collections.Generic;

public class PropsStateController : StateController
{
    [Header("Settings")]
    [SerializeField]
    private float _propsSpacing = 2f;

    [Header("Props Holder")]
    [SerializeField]
    private Transform _propsHolder;

    [Header("Props")]
    [SerializeField]
    private List<GameObject> _currentProps;

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
        GameObject[] props = maskData.GetProps();
        int count = props.Length;

        for (int i = 0; i < count; i++)
        {
            GameObject prop = Instantiate(props[i], _propsHolder);

            // 세로 배치, 간격 2, 가운데 정렬 (위에서 아래로 0, 1, 2...)
            float y = (count - 1) * _propsSpacing * 0.5f - i * _propsSpacing;
            prop.transform.localPosition = new Vector3(0f, y, 0f);
        }
    }
}
