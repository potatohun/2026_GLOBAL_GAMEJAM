using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneHandler : IGameHandler
{
    private readonly GameManager gm;
    public CutSceneHandler(GameManager gm) { this.gm = gm; }

    public void Enter()
    {
        // START에 들어왔다는 건 씬 로드 직후/직전일 수 있음
        // 보통은 씬 로드 후 WAITING으로 넘기는 게 자연스러움
        Debug.Log("ENTER CUTSCENE");
        SceneManager.LoadScene(gm.cutScene);
        // 예: 씬 로드 끝나면 WAITING으로 넘어가고 싶으면

        // 씬 로드 완료 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CutSceneManager _cutSceneManager;
        // 씬 로드 후 UI_Manager 재탐색
        _cutSceneManager = UnityEngine.Object.FindAnyObjectByType<CutSceneManager>();

        if (_cutSceneManager != null) { 
            _cutSceneManager.StartCutScene();
        }

    }

    public void Tick() { }
    public void Exit() { }
}
