using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum MaskCreateState
{
    Request = 0,
    Shape = 1,
    Paint = 2,
    Props = 3,
    Result = 4,
}

public class MaskCreateManager : MonoBehaviour
{
    public static MaskCreateManager instance;

    [Header("State Controllers")]
    [SerializeField]
    public List<StateController> stateControllers;

    [Header("CinemachineCameraController")]
    [SerializeField]
    public CinemachineCameraController cinemachineCameraController;

    [Header("Current State")]
    [SerializeField]
    private MaskCreateState _currentState;

    [Header("Mask Prefab")]
    [SerializeField]
    public GameObject _maskPrefab;

    [Header("Mask Holder")]
    [SerializeField]
    public Transform _maskHolder;

    [Header("Mask")]
    [SerializeField]
    private GameObject _currentMask;

    [Header("Mask Controller")]
    [SerializeField]
    private MaskController _currentMaskController;

    private MaskData _currentMaskData;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _currentState = MaskCreateState.Request;
    }

    void Start()
    {
        // 테스트 용 시작
        Play();
    }

    public void Init()
    {
        // 초기화
        for (int i = 0; i < _maskHolder.childCount; i++)
            Destroy(_maskHolder.GetChild(i).gameObject);

        if (_currentMask != null)
            _currentMask = null;

        if (_currentMaskController != null)
            _currentMaskController = null;

        if (_currentMaskData != null)
            _currentMaskData = null;

        _currentState = MaskCreateState.Request;
    }

    public void Play()
    {
        // Play 시작
        SetCameraToCurrentState();
    }

    public void Next()
    {
        // 상태 컨트롤러 탈출
        int index = (int)_currentState;
        stateControllers[index].OnExitState();

        // 상태 컨트롤러 진입
        int nextIndex = index + 1;
        if (nextIndex > (int)MaskCreateState.Result)
            nextIndex = (int)MaskCreateState.Request;

        // 상태 변경
        _currentState = (MaskCreateState)nextIndex;
        SetCameraToCurrentState();
    }

    void SetCameraToCurrentState()
    {
        if (cinemachineCameraController == null || stateControllers == null)
            return;

        int index = (int)_currentState;
        if (index < 0 || index >= stateControllers.Count)
            return;

        // 카메라 타겟 설정
        cinemachineCameraController.SetTarget(stateControllers[index].transform);

        // 상태 컨트롤러 진입
        stateControllers[index].OnEnterState();
    }

    public void SetMask(MaskData maskData)
    {
        // 마스크 데이터 생성
        _currentMaskData = maskData;

        // 마스크 생성 및 위치 초기화
        _currentMask = Instantiate(_maskPrefab);
        _currentMask.transform.SetParent(_maskHolder);
        _currentMask.transform.localPosition = new Vector3(0, 0, 1);
        _currentMask.transform.localRotation = Quaternion.identity;
        _currentMask.transform.localScale = Vector3.one;

        _currentMaskController = _currentMask.GetComponent<MaskController>();

        // 각 단계 별 필요 정보 전달
    }

    public MaskController GetCurrentMaskController()
    {
        return _currentMaskController;
    }

    public MaskCreateState GetCurrentState()
    {
        return _currentState;
    }

    public MaskData GetCurrentMaskData()
    {
        return _currentMaskData;
    }

    public Transform GetMaskHolder()
    {
        return _maskHolder;
    }
}
