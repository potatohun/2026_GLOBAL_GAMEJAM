using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
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
        //if (gm._gameUI) gm._gameUI = GameUI.i ? GameUI.i : FindAnyObjectByType<GameUI>();
        gm.StartLimitTimer();   // 제한시간 시작 같은 거 여기서
        GameState.i.SetState(GAME.WAITING);
    }

    public void Tick() { }
    public void Exit() { }
}
