using UnityEngine;

[CreateAssetMenu(fileName = "MaskData", menuName = "Scriptable Objects/MaskData")]
public class MaskData : ScriptableObject
{
    [Header("Mask ID")]
    [SerializeField] string mask_id;

    [Header("Target Mask Sprite")]
    [SerializeField] Sprite target_mask_sprite;

    [Header("Color Palette")]
    [SerializeField] Color[] color_palette;

    [Header("Mask Texture")]
    [SerializeField] GameObject[] props;

    public string GetMaskId()
    {
        return mask_id;
    }

    public Sprite GetTargetMaskSprite()
    {
        return target_mask_sprite;
    }

    public Color[] GetColorPalette()
    {
        return color_palette;
    }

    public GameObject[] GetProps()
    {
        return props;
    }
}