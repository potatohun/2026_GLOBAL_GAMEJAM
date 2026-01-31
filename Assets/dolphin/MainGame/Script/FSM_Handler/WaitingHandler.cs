using UnityEngine;
using UnityEngine.SceneManagement;
public class WaitingHandler : IGameHandler
{
    private readonly GameManager gm;
    public WaitingHandler(GameManager gm) { this.gm = gm; }

    public void Enter()
    {
        Debug.Log("ENTER WAITING");
    }

    public void Tick()
    {
        // 예: 입력/조건 만족하면 CONTACT로
        // if (조건) GameState.i.SetState(GAME.CONTACT);
    }

    public void Exit()
    {
        // 상태를 나갈 때 필요한 정리
        Debug.Log("EXIT WAITING");
    }
}