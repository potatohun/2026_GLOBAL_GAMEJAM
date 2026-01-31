using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class EraseMaskController : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public Collider2D targetCollider;

    [Header("Materials")]
    public Material displayMaterial;        // Shader: Unlit/SpriteEraseDisplay (수정본)
    public Material eraseStampMaterial;     // Shader: Hidden/EraseStamp
    public Material paintStampMaterial;     // Shader: Hidden/PaintStamp (추가)

    [Header("Brush")]
    [Range(0.001f, 0.5f)] public float radiusUV = 0.04f;
    [Range(0.0f, 1.0f)] public float hardness = 0.8f;
    [Range(0.05f, 1.0f)] public float spacingFactor = 0.35f;

    [Header("Paint")]
    public Color paintColor = Color.red;    // “선택한 색상” (UI에서 바꾸면 됨)
    [Range(0.0f, 1.0f)] public float paintOpacity = 1.0f; // 1이면 진하게, 0.5면 반투명

    [Header("Limit to sprite pixels (optional)")]
    public bool limitToSpriteAlpha = true;
    [Range(0.0f, 1.0f)] public float alphaThreshold = 0.05f;

    [Header("RT Quality")]
    public int downscale = 1;
    public FilterMode filterMode = FilterMode.Bilinear;

    // Renderer
    private SpriteRenderer sr;

    // Mask RT (erase)
    private RenderTexture maskRT;
    private RenderTexture maskTempRT;

    // Paint RT (color)
    private RenderTexture paintRT;
    private RenderTexture paintTempRT;

    // Material instances (object-specific)
    private Material displayMat;
    private Material eraseStampMat;
    private Material paintStampMat;

    // Stroke smoothing
    private bool hasLastUV;
    private Vector2 lastUV;

    // Shader property IDs
    static readonly int MaskTexId = Shader.PropertyToID("_MaskTex");
    static readonly int PaintTexId = Shader.PropertyToID("_PaintTex");

    static readonly int CenterId = Shader.PropertyToID("_CenterUV");
    static readonly int RadiusId = Shader.PropertyToID("_Radius");
    static readonly int HardId = Shader.PropertyToID("_Hardness");

    static readonly int BaseTexId = Shader.PropertyToID("_BaseTex");
    static readonly int AlphaThId = Shader.PropertyToID("_AlphaThreshold");
    static readonly int UseLimitId = Shader.PropertyToID("_UseAlphaLimit");

    static readonly int BrushColorId = Shader.PropertyToID("_BrushColor");
    static readonly int OpacityId = Shader.PropertyToID("_Opacity");
    static readonly int EraseMaskId = Shader.PropertyToID("_EraseMask");

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!cam) cam = Camera.main;
        if (!targetCollider) targetCollider = GetComponent<Collider2D>();

        // 인스턴스화(오브젝트별 독립)
        displayMat = Instantiate(displayMaterial);
        eraseStampMat = Instantiate(eraseStampMaterial);
        paintStampMat = Instantiate(paintStampMaterial);

        CreateRTs();
        BindMaterials();
    }

    void OnDestroy()
    {
        ReleaseRT(maskRT);
        ReleaseRT(maskTempRT);
        ReleaseRT(paintRT);
        ReleaseRT(paintTempRT);

        if (displayMat) Destroy(displayMat);
        if (eraseStampMat) Destroy(eraseStampMat);
        if (paintStampMat) Destroy(paintStampMat);
    }

    void ReleaseRT(RenderTexture rt)
    {
        if (!rt) return;
        rt.Release();
    }

    void CreateRTs()
    {
        var tex = sr.sprite.texture;
        int w = Mathf.Max(8, tex.width / Mathf.Max(1, downscale));
        int h = Mathf.Max(8, tex.height / Mathf.Max(1, downscale));

        // mask (erase)
        maskRT = NewRT(w, h);
        maskTempRT = NewRT(w, h);

        // paint (color)
        paintRT = NewRT(w, h);
        paintTempRT = NewRT(w, h);

        // init: mask=black(0), paint=transparent(0,0,0,0)
        var prev = RenderTexture.active;

        RenderTexture.active = maskRT; GL.Clear(true, true, Color.black);
        RenderTexture.active = maskTempRT; GL.Clear(true, true, Color.black);

        RenderTexture.active = paintRT; GL.Clear(true, true, new Color(0, 0, 0, 0));
        RenderTexture.active = paintTempRT; GL.Clear(true, true, new Color(0, 0, 0, 0));

        RenderTexture.active = prev;
    }

    RenderTexture NewRT(int w, int h)
    {
        var rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
        rt.filterMode = filterMode;
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.useMipMap = false;
        rt.Create();
        return rt;
    }

    void BindMaterials()
    {
        // Display: mask + paint 연결
        displayMat.SetTexture(MaskTexId, maskRT);
        displayMat.SetTexture(PaintTexId, paintRT);
        sr.material = displayMat;

        // Erase stamp: 투명 픽셀 제한 옵션(원하면 끄기)
        eraseStampMat.SetTexture(BaseTexId, sr.sprite.texture);
        eraseStampMat.SetFloat(AlphaThId, alphaThreshold);
        eraseStampMat.SetFloat(UseLimitId, limitToSpriteAlpha ? 1f : 0f);

        // Paint stamp: paintRT 누적 + (중요) erase mask를 읽어서 “지워지지 않은 영역”에만 그림
        paintStampMat.SetTexture(EraseMaskId, maskRT);
        paintStampMat.SetFloat(OpacityId, paintOpacity);
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null) return;

        bool mouseDown = Mouse.current.leftButton.isPressed;
        if (!mouseDown)
        {
            hasLastUV = false;
            return;
        }

        // A 키를 누르는 동안은 Paint 모드 (안 누르면 Erase 모드)
        bool paintMode = Keyboard.current.aKey.isPressed;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        // 스프라이트 영역 안에서만
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
        if (!hit || hit.collider != targetCollider)
        {
            hasLastUV = false;
            return;
        }

        if (!TryWorldToSpriteUV(hit.point, out Vector2 uv))
        {
            hasLastUV = false;
            return;
        }

        // 첫 프레임
        if (!hasLastUV)
        {
            if (paintMode) PaintAtUV(uv);
            else EraseAtUV(uv);

            lastUV = uv;
            hasLastUV = true;
            return;
        }

        // 점선 방지(보간)
        StepAlongSegment(lastUV, uv, paintMode);

        lastUV = uv;
    }

    void StepAlongSegment(Vector2 from, Vector2 to, bool paintMode)
    {
        float dist = Vector2.Distance(from, to);
        float step = Mathf.Max(0.0001f, radiusUV * spacingFactor);
        int count = Mathf.Clamp(Mathf.CeilToInt(dist / step), 1, 128);

        for (int i = 1; i <= count; i++)
        {
            float t = (float)i / count;
            Vector2 p = Vector2.Lerp(from, to, t);
            if (paintMode) PaintAtUV(p);
            else EraseAtUV(p);
        }
    }

    void EraseAtUV(Vector2 uv)
    {
        eraseStampMat.SetVector(CenterId, uv);
        eraseStampMat.SetFloat(RadiusId, radiusUV);
        eraseStampMat.SetFloat(HardId, hardness);

        // mask ping-pong
        Graphics.Blit(maskRT, maskTempRT, eraseStampMat);
        Swap(ref maskRT, ref maskTempRT);

        // paintStamp가 최신 마스크를 보도록 갱신
        paintStampMat.SetTexture(EraseMaskId, maskRT);

        // display 갱신
        displayMat.SetTexture(MaskTexId, maskRT);
    }

    void PaintAtUV(Vector2 uv)
    {
        paintStampMat.SetVector(CenterId, uv);
        paintStampMat.SetFloat(RadiusId, radiusUV);
        paintStampMat.SetFloat(HardId, hardness);
        paintStampMat.SetColor(BrushColorId, paintColor);
        paintStampMat.SetFloat(OpacityId, paintOpacity);

        // paint ping-pong
        Graphics.Blit(paintRT, paintTempRT, paintStampMat);
        Swap(ref paintRT, ref paintTempRT);

        // display 갱신
        displayMat.SetTexture(PaintTexId, paintRT);
    }

    void Swap(ref RenderTexture a, ref RenderTexture b)
    {
        var tmp = a; a = b; b = tmp;
    }

    bool TryWorldToSpriteUV(Vector2 worldPoint, out Vector2 uv)
    {
        uv = default;

        Vector3 local = transform.InverseTransformPoint(worldPoint);
        Bounds b = sr.sprite.bounds;

        float u = Mathf.InverseLerp(b.min.x, b.max.x, local.x);
        float v = Mathf.InverseLerp(b.min.y, b.max.y, local.y);

        if (u < 0 || u > 1 || v < 0 || v > 1) return false;

        uv = new Vector2(u, v);
        return true;
    }
}
