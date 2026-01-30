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

    void Start() {
        // ####### 테스트 코드 #######
        this.SetBaseSprite(this._baseSpriteRenderer);
        this.SetTargetSprite(this._targetSpriteRenderer);
        
        float result = Compare();
        Debug.Log($"유사도: {result:F1}%");
    }

    public void SetBaseSprite(SpriteRenderer spriteRenderer) {
        this._baseSpriteRenderer = spriteRenderer;
        this._baseSprite = spriteRenderer.sprite;
    }

    public void SetTargetSprite(SpriteRenderer spriteRenderer) {
        this._targetSpriteRenderer = spriteRenderer;
        this._targetSprite = spriteRenderer.sprite;
    }

    float Compare() {
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

        float sumSimilarity = 0f;
        int count = 0;

        for (int y = 0; y < compareH; y++) {
            for (int x = 0; x < compareW; x++) {
                Color c1 = this._baseSprite.texture.GetPixel(xMin1 + x, yMin1 + y);
                Color c2 = this._targetSprite.texture.GetPixel(xMin2 + x, yMin2 + y);

                float diff = Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g)
                    + Mathf.Abs(c1.b - c2.b) + Mathf.Abs(c1.a - c2.a);
                float pixelSimilarity = 1f - (diff / 4f);
                sumSimilarity += pixelSimilarity;
                count++;
            }
        }

        if (count == 0) 
        {
            Debug.LogWarning("Count is 0.");
            return -1f;
        }

        return (sumSimilarity / count) * 100f;
    }
}
