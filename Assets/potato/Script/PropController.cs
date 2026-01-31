using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PropController : MonoBehaviour
{
    private Camera _camera;
    private bool _isUsed = false;

    void Awake()
    {
        _isUsed = false;
        if (_camera == null)
            _camera = Camera.main;
    }

    public void OnDrag()
    {
        if (_isUsed)
            return;

        if (_camera == null || Mouse.current == null)
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 pos = new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_camera.transform.position.z - transform.position.z));
        Vector3 worldPos = _camera.ScreenToWorldPoint(pos);
        worldPos.z = transform.position.z;
        transform.position = worldPos;
    }

    public void OnDrop()
    {
        Debug.Log("OnDrop");
        _isUsed = true;

        Transform maskHolder = MaskCreateManager.instance.GetMaskHolder();
        this.transform.SetParent(maskHolder);

        Debug.Log("transform: " + this.transform.parent.name);
    }
}
