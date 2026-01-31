using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public interface IGameHandler
{
    void Enter();
    void Tick();
    void Exit();
}

public class GameManager : MonoBehaviour
{
    public static GameManager i { get; private set; }
    [SerializeField] private GameState state;

    private Dictionary<GAME, IGameHandler> handlers;
    private IGameHandler currentHandler;

    public string gameScene;
    [SerializeField] private float Limittime;

    public GameUI _gameUI;

    void Awake()
    {
        if (i != null) { Destroy(gameObject); return; }
        i = this;
        DontDestroyOnLoad(gameObject);

        if (!state) state = GameState.i ? GameState.i : FindAnyObjectByType<GameState>();

        handlers = new Dictionary<GAME, IGameHandler>
        {
            { GAME.MENU,    new MenuHandler(this)},
            { GAME.START,   new StartHandler(this) },
            { GAME.WAITING, new WaitingHandler(this) },
            { GAME.CONTACT, new ContactHandler(this) },
            { GAME.MAKING,  new MakingHandler(this) },
            { GAME.RESULT,  new ResultHandler(this) },
            { GAME.END,     new EndHandler(this) },
        };

    }


    private void OnEnable()
    {
        if (!state) state = GameState.i ? GameState.i : FindAnyObjectByType<GameState>();
        if (state) state.OnStateChanged += OnStateChanged;

        // 현재 상태가 이미 세팅돼있을 수 있으니 Enter 한번 보장
        if (state) SwitchTo(state.PlayState);
    }

    private void OnDisable()
    {
        if (state) state.OnStateChanged -= OnStateChanged;
    }

    private void Update()
    {
        currentHandler?.Tick();
    }

    private void OnStateChanged(GAME prev, GAME next)
    {
        SwitchTo(next);
    }

    private void SwitchTo(GAME next)
    {
        currentHandler?.Exit();

        if (!handlers.TryGetValue(next, out currentHandler))
        {
            Debug.LogError($"Handler not found for state: {next}");
            currentHandler = null;
            return;
        }

        currentHandler.Enter();
    }

    // ===== 상태에서 호출할 공개 기능(여기 모아두면 책임 분리 깔끔) =====

    public float GetLimitTime() => i ? i.Limittime : 0f;

    public void StartLimitTimer()
    {
        // TODO: 타이머 컴포넌트 Start, UI 표시 등
        Debug.Log($"Timer Start: {GetLimitTime()} sec");
        _gameUI.Start_Timer();
    }

    public void StopLimitTimer()
    {
        // TODO: 타이머 Stop
        Debug.Log("Timer Stop");
        _gameUI.Stop_Tmer();
    }

    public void GoResult()
    {
        state.SetState(GAME.RESULT);
    }

    public void GoEnd()
    {
        state.SetState(GAME.END);
    }
}

