using UnityEngine;
using DG.Tweening;
using System.Collections;

public class RequestStateController : StateController
{
    public Transform _cameraTarget;

    public RequestPanelController _requestPanel;
    public override void OnEnterState()
    {
        this.transform.DOMoveY(-11f, 1f).SetEase(Ease.InOutExpo);

        // 초기화
        MaskCreateManager.instance.Init();

        // 손님 기다리기
        CinemachineCameraController.instance.SetTarget(_cameraTarget);

        StartCoroutine(RequestCoroutine());
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

    IEnumerator RequestCoroutine()
    {
        yield return new WaitForSeconds(3f);

        // 새로운 MaskData 생성
        MaskData maskData = MaskDataList.instance.GetRandomMaskData();
        MaskCreateManager.instance.SetMask(maskData);

        // 미리보기 이미지 설정
        InGameUIController.instance.SetPreviewImage(maskData.GetBaseMaskSprite());
        InGameUIController.instance.SetPreviewPanel(false);
        InGameUIController.instance.SetResultPanel(false);
        InGameUIController.instance.SetNextButton(false);

        // 요청 이미지 설정
        _requestPanel.SetRequestImage(maskData.GetBaseMaskSprite());
        _requestPanel.Open();

        AudioManager.instance.PlayBirdCome();
    }
}
