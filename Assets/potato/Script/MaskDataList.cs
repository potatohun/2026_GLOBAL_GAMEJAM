using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MaskDataPerLevel
{
    public int level;
    public List<MaskData> maskDataList;
}
public class MaskDataList : MonoBehaviour
{
    public static MaskDataList instance;

    [SerializeField]
    public List<MaskDataPerLevel> maskDataPerLevelList;

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
        int currentLevel = PlayerPrefs.GetInt("level");
        if(currentLevel == null)
            currentLevel = 0;
        
        switch (currentLevel)
        {
            case 0:
                return maskDataPerLevelList[0].maskDataList[Random.Range(0, maskDataPerLevelList[0].maskDataList.Count)];
            case 1:
                return maskDataPerLevelList[1].maskDataList[Random.Range(0, maskDataPerLevelList[1].maskDataList.Count)];
            case 2:
                return maskDataPerLevelList[2].maskDataList[Random.Range(0, maskDataPerLevelList[2].maskDataList.Count)];
            default:
                return maskDataPerLevelList[0].maskDataList[Random.Range(0, maskDataPerLevelList[0].maskDataList.Count)];
        }
    }

    public MaskData GetMaskData(int index)
    {
        return maskDataList[index];
    }
}
