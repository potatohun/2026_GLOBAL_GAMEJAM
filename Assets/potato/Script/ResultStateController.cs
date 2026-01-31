using System.Collections;
using System.IO;
using UnityEngine;

public class ResultStateController : StateController
{
    [Header("Base")]
    [SerializeField] private SpriteRenderer _baseSpriteRenderer;

    [Header("Target")]
    [SerializeField] private SpriteRenderer _targetSpriteRenderer;

    [Header("Texture Compare")]
    [SerializeField] private TextureCompareController _textureCompareController;
    
    [Header("Capture")]
    [SerializeField] private float _enterDelay = 0f;
    [SerializeField] private Camera _captureCamera;
    [SerializeField] private Transform _captureTarget;
    [SerializeField] private int _captureSize = 512;
    [SerializeField] private float _captureOrthoSize = 5f;

    public override void OnEnterState()
    {
        // 초기화
        Init();

        // 캡처
        StartCoroutine(EnterStateDelayed());
    }

    private void Init()
    {
        _baseSpriteRenderer.sprite = MaskCreateManager.instance.GetCurrentMaskData().GetTargetMaskSprite();
    }

    IEnumerator EnterStateDelayed()
    {
        if (_enterDelay > 0f)
            yield return new WaitForSeconds(_enterDelay);

        // Base Texture 캡쳐
        Texture2D baseTexture = CaptureAt(_baseSpriteRenderer.transform);
        if (baseTexture == null)
        {
            Debug.LogWarning("Base Texture is null. Cannot capture.");
            yield break;
        }

        Rect baseRect = new Rect(0, 0, baseTexture.width, baseTexture.height);
        Sprite baseSprite = Sprite.Create(baseTexture, baseRect, new Vector2(0.5f, 0.5f));
        _baseSpriteRenderer.sprite = baseSprite;

        SaveCapture(baseTexture, "base.png");

        // Target Texture 캡쳐쳐
        Texture2D targetTexture = CaptureAt(_captureTarget);
        if(targetTexture == null){
            Debug.LogWarning("Target Texture is null. Cannot capture.");
            yield break;
        }

        Rect targetRect = new Rect(0, 0, targetTexture.width, targetTexture.height);
        Sprite targetSprite = Sprite.Create(targetTexture, targetRect, new Vector2(0.5f, 0.5f));
        _targetSpriteRenderer.sprite = targetSprite;

        SaveCapture(targetTexture, "target.png");
        
        // 텍스처 비교
        _textureCompareController.SetBaseSprite(_baseSpriteRenderer);
        _textureCompareController.SetTargetSprite(_targetSpriteRenderer);

        float similarity = _textureCompareController.Compare();
        Debug.Log($"텍스처 비교 결과: {similarity}");
    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }

    public Texture2D CaptureScreen()
    {
        if (_captureCamera == null)
            return null;

        int size = Mathf.Clamp(_captureSize, 512, 512);
        RenderTexture rt = RenderTexture.GetTemporary(size, size, 24);
        RenderTexture prev = _captureCamera.targetTexture;

        _captureCamera.targetTexture = rt;
        _captureCamera.Render();
        _captureCamera.targetTexture = prev;

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(size, size);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return tex;
    }

    public Texture2D CaptureAt(Transform target)
    {
        if (target == null || _captureCamera == null)
        {
            Debug.LogWarning("Target or Capture Camera is null. Cannot capture.");
            return null;
        }

        Vector3 prevPos = _captureCamera.transform.position;
        Quaternion prevRot = _captureCamera.transform.rotation;
        float prevOrthoSize = 0f;
        bool wasOrtho = _captureCamera.orthographic;

        if (_captureCamera.orthographic)
        {
            prevOrthoSize = _captureCamera.orthographicSize;
            _captureCamera.orthographicSize = _captureOrthoSize;
            Vector3 p = target.position;
            _captureCamera.transform.position = new Vector3(p.x, p.y, prevPos.z);
            _captureCamera.transform.rotation = Quaternion.identity;
        }
        else
        {
            _captureCamera.transform.position = target.position + Vector3.back * _captureOrthoSize;
            _captureCamera.transform.LookAt(target.position);
        }

        Texture2D tex = CaptureScreen();

        _captureCamera.transform.position = prevPos;
        _captureCamera.transform.rotation = prevRot;
        if (wasOrtho)
            _captureCamera.orthographicSize = prevOrthoSize;

        return tex;
    }

    public void SaveCapture(Texture2D texture = null, string fileName = "capture")
    {
        if (texture == null)
        {
            Debug.LogWarning("Texture is null. Cannot save capture.");
            return;
        }

        if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
            fileName += ".png";

        string dir = Application.persistentDataPath;
        string path = Path.Combine(dir, fileName);
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        Debug.Log($"캡처 저장됨: {path}");
    }
}
