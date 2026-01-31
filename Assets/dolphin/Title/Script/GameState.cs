using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.CullingGroup;

public enum GAME
{
    MENU,
    START,
    WAITING,
    CONTACT,
    MAKING,
    RESULT,
    END,
};


public class GameState : MonoBehaviour
{
    public static GameState i;
    public GAME PlayState { get; private set; }

    public event Action<GAME, GAME> OnStateChanged;

    public int level = 0;
    public float conquer = 50.0f;


    private void Awake()
    {
        if (i != null && i != this)
        {
            Destroy(gameObject);
            return;
        }

        i = this;
        LoadGameData();
        DontDestroyOnLoad(gameObject);

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Debug.LogError("Unhandled Exception 발생 - 무시하고 계속 실행");
        };
    }

    public void SetState(GAME next)
    {
        if (PlayState == next) return;
        var prev = PlayState;
        PlayState = next;
        OnStateChanged?.Invoke(prev, next);
    }

    public void SaveGameData(float remainCounquer, float AddCounquer)
    {
        level += 1;
        conquer = remainCounquer + AddCounquer;

        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("Conquer", conquer);
        Debug.Log( "[SaveGame]"+" "+ "level : " +  level + " " + "Conquer : " + conquer);
    }

    public void LoadGameData()
    {
        if (!PlayerPrefs.HasKey("level")) PlayerPrefs.SetInt("level", 0);
        if (!PlayerPrefs.HasKey("Conquer")) PlayerPrefs.SetFloat("Conquer", 50.0f);
        level = PlayerPrefs.GetInt("level");
        conquer = PlayerPrefs.GetFloat("Conquer");
        Debug.Log("[LoadGame]" + " " + "level : " + level + " " + "Conquer : " + conquer);
    }
}
