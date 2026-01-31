using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    private CinemachineCamera _cinemachineCamera;

    void Awake()
    {
        this._cinemachineCamera = this.GetComponent<CinemachineCamera>();
    }

    public void SetTarget(Transform target)
    {
        this._cinemachineCamera.Follow = target;
    }
}
