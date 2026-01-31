using UnityEngine;

public class RequestStateController : StateController
{
    public override void OnEnterState()
    {
        // 손님 기다리기 

        // 손님 기다리기 완료
        
        // 새로운 MaskData 생성
        MaskData maskData = MaskDataList.instance.GetRandomMaskData();
        MaskCreateManager.instance.SetMask(maskData);
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
