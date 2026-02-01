using UnityEngine;

public class TextureCompareController : MonoBehaviour
{
    [Header("Compare Settings")]
    [SerializeField]
    private int _horizontalTolerance = 0;

    [SerializeField]
    private int _verticalTolerance = 0;

    [SerializeField]
    [Tooltip("비교 전 다운샘플링 크기 (512x512 → 이 값으로 리사이즈)")]
    private int _downSampleSize = 100;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("이 값 미만의 alpha는 비교 대상에서 제외 (투명 처리)")]
    private float _alphaThreshold = 0.1f;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("색상(RGB) 차이가 이 값 미만이면 매칭으로 인정")]
    private float _colorThreshold = 0.1f;

    /// <summary>
    /// 텍스쳐를 targetSize x targetSize로 다운샘플링 (포인트 샘플링)
    /// </summary>
    private Texture2D DownSample(Texture2D source, int targetSize)
    {
        targetSize = Mathf.Clamp(targetSize, 4, 512);
        Texture2D result = new Texture2D(targetSize, targetSize);
        Color[] srcPixels = source.GetPixels();

        for (int y = 0; y < targetSize; y++)
        {
            for (int x = 0; x < targetSize; x++)
            {
                float u = (x + 0.5f) / targetSize;
                float v = (y + 0.5f) / targetSize;
                int srcX = Mathf.Clamp(Mathf.FloorToInt(u * source.width), 0, source.width - 1);
                int srcY = Mathf.Clamp(Mathf.FloorToInt(v * source.height), 0, source.height - 1);
                int idx = srcY * source.width + srcX;
                result.SetPixel(x, y, srcPixels[idx]);
            }
        }
        result.Apply();
        return result;
    }

    public float Compare(Texture2D baseSprite, Texture2D targetSprite) {
        Texture2D baseTexture = baseSprite;
        Texture2D targetTexture = targetSprite;
        if (baseTexture == null || targetTexture == null || !baseTexture.isReadable || !targetTexture.isReadable) 
        {
            Debug.LogWarning("Base Texture or Target Texture is not readable.");
            return -1f;
        }

        // 512x512 등 큰 텍스처를 100x100으로 다운샘플링하여 비교
        Texture2D baseDown = DownSample(baseTexture, _downSampleSize);
        Texture2D targetDown = DownSample(targetTexture, _downSampleSize);

        int compareW = baseDown.width;
        int compareH = baseDown.height;

        Debug.Log($"Base: {baseTexture.width}x{baseTexture.height} → {compareW}x{compareH}, Target: {targetTexture.width}x{targetTexture.height} → {compareW}x{compareH}");

        if (compareW <= 0 || compareH <= 0) 
        {
            Debug.LogWarning("Compare Width or Compare Height is less than 0.");
            Object.Destroy(baseDown);
            Object.Destroy(targetDown);
            return -1f;
        }

        Color[] basePixels = baseDown.GetPixels();
        Color[] targetPixels = targetDown.GetPixels();

        int matchCount = 0;
        int count = 0;

        for (int y = 0; y < compareH; y++) {
            for (int x = 0; x < compareW; x++) {
                Color c1 = basePixels[y * compareW + x];

                if (c1.a < _alphaThreshold)
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

                        if (c2.a < _alphaThreshold)
                            continue;

                        if (Mathf.Abs(c1.r - c2.r) < _colorThreshold
                         && Mathf.Abs(c1.g - c2.g) < _colorThreshold
                         && Mathf.Abs(c1.b - c2.b) < _colorThreshold
                         && Mathf.Abs(c1.a - c2.a) < _colorThreshold)
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
            Object.Destroy(baseDown);
            Object.Destroy(targetDown);
            return -1f;
        }

        float result = ((float)matchCount / count) * 100f;
        Object.Destroy(baseDown);
        Object.Destroy(targetDown);
        return result;
    }
}
