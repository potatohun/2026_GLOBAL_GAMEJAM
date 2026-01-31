using UnityEngine;

public class StateController : MonoBehaviour
{
    public virtual void OnEnterState() { }
    public virtual void OnUpdateState() { }
    public virtual void OnExitState() { }
}
