using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public string cutScene;
    [SerializeField] private float Limittime;

    public GameUI _gameUI;

    public Dictionary<Sprite, float> SuccessAgentDic;
    public Dictionary<Sprite, float> FailAgentDic;

    public int Tal_cnt = 0;
    public float Max_Score = 0.0f;

    void Awake()
    {
        if (i != null) { Destroy(gameObject); return; }
        i = this;
        DontDestroyOnLoad(gameObject);

        // Dictionary ???
        SuccessAgentDic = new Dictionary<Sprite, float>();
        FailAgentDic = new Dictionary<Sprite, float>();

        if (!state) state = GameState.i ? GameState.i : FindAnyObjectByType<GameState>();

        handlers = new Dictionary<GAME, IGameHandler>
        {
            { GAME.MENU,    new MenuHandler(this)},
            { GAME.CUTSCENE, new CutSceneHandler(this)},
            { GAME.START,   new StartHandler(this) },
            { GAME.WAITING, new WaitingHandler(this) },
            { GAME.CONTACT, new ContactHandler(this) },
            { GAME.MAKING,  new MakingHandler(this) },
            { GAME.RESULT,  new ResultHandler(this) },
            { GAME.END,     new EndHandler(this) },
        };

    }

    public void ResetScore()
    {
        Tal_cnt = 0;
        Max_Score = 0.0f;

        SuccessAgentDic.Clear();
        FailAgentDic.Clear();
    }

    private void OnEnable()
    {
        if (!state) state = GameState.i ? GameState.i : FindAnyObjectByType<GameState>();
        if (state) state.OnStateChanged += OnStateChanged;

        // ???? ???�? ??? ???�????? ?? ?????? Enter ??? ????
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

    // ===== ???�??? ????? ???? ???(???? ????? �?? ??? ???) =====

    public float GetLimitTime() => i ? i.Limittime : 0f;

    public void StartLimitTimer()
    {
        // TODO: ???? ??????? Start, UI ??? ??
        Debug.Log($"Timer Start: {GetLimitTime()} sec");
        _gameUI.Start_Timer();
    }

    public void StopLimitTimer()
    {
        // TODO: ???? Stop
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

    public void SentAgent(Sprite sprite, float probability)
    {
        Tal_cnt++;
        CheckProbability(probability);

        float AgentProbability = probability + 10.0f;

        float MissionProbability = Random.Range(0.0f, 1.0f) * 100.0f;

        if(AgentProbability >= MissionProbability)
        {
            UpdateSuccessAgent(sprite, probability);
        }
        else
        {
            UpdateFailAgent(sprite, probability);
        }
    }

    private void UpdateSuccessAgent(Sprite sprite, float probability)
    {
        SuccessAgentDic.Add(sprite, probability);
        Debug.Log($"SuccessAgentDic: {sprite.name} - {probability}");
    }

    private void UpdateFailAgent(Sprite sprite, float probability)
    {
        FailAgentDic.Add(sprite, probability);
    }

    private void CheckProbability(float probability)
    {
        float Score = 0;

        if(probability >= 70.0f)
        {
            Score = 12;
        }
        else if(probability >= 60.0f)
        {
            Score = 6;
        }
        else if(probability >= 50.0f)
        {
            Score = 3;
        }
        else if(probability >= 30.0f)
        {
            Score = -6;
        }
        else if(probability >= 10.0f)
        {
            Score = -12;
        }
        else
        {
            Score = -24;
        }
        _gameUI.UpdateConquer(Score);
    }
}

