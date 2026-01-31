using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PropController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _maskLayer;

    private bool _isUsed = false;

    void Awake()
    {
        _isUsed = false;
        if (_camera == null)
            _camera = Camera.main;
        if (_maskLayer == 0)
            _maskLayer = LayerMask.GetMask("Mask");
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

    public void OnDragEnd()
    {
        Debug.Log("OnDrop");
        _isUsed = true;

        // 마지막 마우스 포인터 위치가 Mask Layer 위에 있는지 Raycast로 확인
        bool isOnMaskLayer = IsMouseOverMaskLayer();

        Transform maskHolder = MaskCreateManager.instance.GetMaskHolder();
        if (isOnMaskLayer)
        {
            this.transform.SetParent(maskHolder);
            this.gameObject.layer = LayerMask.NameToLayer("Mask");
            Debug.Log("transform: " + this.transform.parent.name + " (Mask 위에 드롭됨)");
        }
        else
        {
            Debug.Log("Mask Layer 위가 아님 - 드롭 무시 또는 다른 처리");
        }
    }

    /// <summary>
    /// 현재 마우스 위치가 Mask Layer 위에 있는지 Raycast로 확인. 자기 자신(Prop)은 제외.
    /// </summary>
    bool IsMouseOverMaskLayer()
    {
        if (_camera == null) _camera = Camera.main;
        if (_camera == null || Mouse.current == null) return false;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos3 = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_camera.transform.position.z)));
        Vector2 worldPos = new Vector2(worldPos3.x, worldPos3.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos, _maskLayer);
        foreach (Collider2D col in hits)
        {
            if (col == null) continue;
            // 자기 자신(이 Prop) 또는 자식 콜라이더는 제외
            if (col.transform.IsChildOf(transform) || col.transform == transform)
                continue;
            return true;
        }
        return false;
    }
}
