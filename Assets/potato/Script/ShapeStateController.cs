using UnityEngine;

public class ShapeStateController : StateController
{
    public Transform _cameraTarget;

    public override void OnEnterState()
    {
        CinemachineCameraController.instance.SetTarget(_cameraTarget);

        InGameUIController.instance.SetPreviewPanel(true);

        CursorManager.instance.EquipTool(CursorType.Knife);
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
