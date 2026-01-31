using UnityEngine;
using System.Collections.Generic;
public class MaskDataList : MonoBehaviour
{
    public static MaskDataList instance;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    [SerializeField]
    public List<MaskData> maskDataList;

    public MaskData GetRandomMaskData()
    {
        return maskDataList[Random.Range(0, maskDataList.Count)];
    }

    public MaskData GetMaskData(int index)
    {
        return maskDataList[index];
    }
}
