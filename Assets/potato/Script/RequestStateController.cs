using UnityEngine;

public class RequestStateController : StateController
{
    public override void OnEnterState()
    {
        // 초기화
        MaskCreateManager.instance.Init();

        // 손님 기다리기

        // 손님 기다리기 완료

        // 새로운 MaskData 생성
        MaskData maskData = MaskDataList.instance.GetRandomMaskData();
        MaskCreateManager.instance.SetMask(maskData);

        // 미리보기 이미지 설정
        InGameUIController.instance.SetPreviewImage(maskData.GetBaseMaskSprite());
        InGameUIController.instance.SetPreviewPanel(true);
        InGameUIController.instance.SetResultPanel(false);
        InGameUIController.instance.SetNextButton(true);
    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }
}
