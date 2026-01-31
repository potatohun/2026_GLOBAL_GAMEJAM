using UnityEngine;

public class ShapeStateController : StateController
{
    [Header("Settings")]
    [SerializeField]
    public int _outlineWidth = 1;

    public override void OnEnterState()
    {
        base.OnEnterState();
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
