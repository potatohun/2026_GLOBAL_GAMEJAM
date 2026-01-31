using UnityEngine;

public class TextureCompareController : MonoBehaviour
{
    [Header("Base Sprite")]
    [SerializeField]
    private SpriteRenderer _baseSpriteRenderer;
    private Sprite _baseSprite;

    [Header("Target Sprite")]
    [SerializeField]
    private SpriteRenderer _targetSpriteRenderer;
    private Sprite _targetSprite;

    [Header("Compare Settings")]
    [Tooltip("가로 위치 보정 범위 (픽셀). base 픽셀 기준 ±이 값만큼 좌우에서 같은 색을 찾음")]
    [SerializeField]
    private int _horizontalTolerance = 0;
    [Tooltip("세로 위치 보정 범위 (픽셀). base 픽셀 기준 ±이 값만큼 위아래에서 같은 색을 찾음")]
    [SerializeField]
    private int _verticalTolerance = 0;

    public void SetBaseSprite(SpriteRenderer spriteRenderer) {
        this._baseSpriteRenderer = spriteRenderer;
        this._baseSprite = spriteRenderer.sprite;
    }

    public void SetTargetSprite(SpriteRenderer spriteRenderer) {
        this._targetSpriteRenderer = spriteRenderer;
        this._targetSprite = spriteRenderer.sprite;
    }

    public float Compare() {
        if (this._baseSprite == null || this._targetSprite == null) 
        {
            Debug.LogWarning("Base Sprite or Target Sprite is null.");
            return -1f;
        }

        Texture2D baseTexture = this._baseSprite.texture;
        Texture2D targetTexture = this._targetSprite.texture;
        if (baseTexture == null || targetTexture == null || !baseTexture.isReadable || !targetTexture.isReadable) 
        {
            Debug.LogWarning("Base Texture or Target Texture is not readable.");
            return -1f;
        }

        Rect r1 = this._baseSprite.rect;
        Rect r2 = this._targetSprite.rect; 
        int w1 = (int)r1.width;
        int h1 = (int)r1.height;
        int w2 = (int)r2.width;
        int h2 = (int)r2.height;

        Debug.Log("Base Texture: " + w1 + "x" + h1);
        Debug.Log("Target Texture: " + w2 + "x" + h2);

        int compareW = Mathf.Min(w1, w2);
        int compareH = Mathf.Min(h1, h2);
        if (compareW <= 0 || compareH <= 0) 
        {
            Debug.LogWarning("Compare Width or Compare Height is less than 0.");
            return -1f;
        }

        int xMin1 = (int)r1.xMin;
        int yMin1 = (int)r1.yMin;
        int xMin2 = (int)r2.xMin;
        int yMin2 = (int)r2.yMin;

        // 전체 픽셀 배열을 한 번에 가져오기 (대폭 최적화)
        Color[] basePixels = baseTexture.GetPixels(xMin1, yMin1, compareW, compareH);
        Color[] targetPixels = targetTexture.GetPixels(xMin2, yMin2, compareW, compareH);

        int matchCount = 0;
        int count = 0;
        const float alphaThreshold = 0.01f;
        const float colorThreshold = 0.01f;

        for (int y = 0; y < compareH; y++) {
            for (int x = 0; x < compareW; x++) {
                Color c1 = basePixels[y * compareW + x];

                if (c1.a < alphaThreshold)
                    continue;

                bool isMatch = false;

                // 범위 최적화: 경계 벗어나지 않는 dy, dx만 계산
                int dyMin = Mathf.Max(-_verticalTolerance, -y);
                int dyMax = Mathf.Min(_verticalTolerance, compareH - 1 - y);
                int dxMin = Mathf.Max(-_horizontalTolerance, -x);
                int dxMax = Mathf.Min(_horizontalTolerance, compareW - 1 - x);

                for (int dy = dyMin; dy <= dyMax && !isMatch; dy++)
                {
                    for (int dx = dxMin; dx <= dxMax; dx++)
                    {
                        int targetX = x + dx;
                        int targetY = y + dy;

                        Color c2 = targetPixels[targetY * compareW + targetX];

                        if (c2.a < alphaThreshold)
                            continue;

                        if (Mathf.Abs(c1.r - c2.r) < colorThreshold
                         && Mathf.Abs(c1.g - c2.g) < colorThreshold
                         && Mathf.Abs(c1.b - c2.b) < colorThreshold
                         && Mathf.Abs(c1.a - c2.a) < colorThreshold)
                        {
                            isMatch = true;
                            break;
                        }
                    }
                }

                if (isMatch)
                    matchCount++;
                count++;
            }
        }

        if (count == 0) 
        {
            Debug.LogWarning("Count is 0.");
            return -1f;
        }

        return ((float)matchCount / count) * 100f;
    }
}
