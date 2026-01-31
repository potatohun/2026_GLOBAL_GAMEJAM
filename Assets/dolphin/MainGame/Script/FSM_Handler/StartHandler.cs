using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartHandler : IGameHandler
{
    private readonly GameManager gm;
    public StartHandler(GameManager gm) { this.gm = gm; }

    public void Enter()
    {
        // START에 들어왔다는 건 씬 로드 직후/직전일 수 있음
        // 보통은 씬 로드 후 WAITING으로 넘기는 게 자연스러움
        Debug.Log("ENTER START");
        SceneManager.LoadScene(gm.gameScene);
        // 예: 씬 로드 끝나면 WAITING으로 넘어가고 싶으면

        // 씬 로드 완료 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 UI_Manager 재탐색
        gm._gameUI = UnityEngine.Object.FindAnyObjectByType<GameUI>();

        if (gm._gameUI == null)
        {
            Debug.LogWarning($"UI_Manager not found in scene: {scene.name}");
        }
        else
        {
            Debug.Log("UI_Manager successfully linked");
            GameState.i.LoadGameData();
            gm._gameUI._controlRateUI.SetGageData(GameState.i.conquer);
            gm.StartLimitTimer();   // 제한시간 시작 같은 거 여기서
            GameState.i.SetState(GAME.WAITING);
        }
    }

    public void Tick() { }
    public void Exit() { }
}
