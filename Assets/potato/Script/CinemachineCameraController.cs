using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    public static CinemachineCameraController instance;
    private CinemachineCamera _cinemachineCamera;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        _cinemachineCamera = this.GetComponent<CinemachineCamera>();
    }

    public void SetTarget(Transform target)
    {
        this._cinemachineCamera.Follow = target;
    }
}
