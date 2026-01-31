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

    [SerializeField]
    public string gameScene;
    [SerializeField] private float Limittime;

    private void Awake()
    {
        if (i != null && i != this)
        {
            Destroy(gameObject);
            return;
        }

        i = this;
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
}
