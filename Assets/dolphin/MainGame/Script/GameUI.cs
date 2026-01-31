using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI i;

    [SerializeField] private GameObject _inGamePlayUI;
    [SerializeField] private TimerUI _timerUI;
    [SerializeField] public ControlRateUI _controlRateUI;
    [SerializeField] private ResultUI _resultUI;
    public GameObject ResultView;
    public GameObject MenuView;
    public GameObject GameOverView;
    ///public GameObject MenuButton;

    [SerializeField]
    private string MainMenuScene_Name;

    private void Awake()
    {
        i = this;
        MenuView.SetActive(false);
        ResultView.SetActive(false);
        GameOverView.SetActive(false);
        _inGamePlayUI.SetActive(true);
        _timerUI.limitTime = GameManager.i.GetLimitTime();
    }

    private void Update()
    {
    }

    public void Start_Timer()
    {
        _timerUI.Resume();
        _controlRateUI.Resume();
    }

    public void Stop_Tmer()
    {
        _timerUI.Pause();
        _controlRateUI.Pause();
    }

    public void ShowView(GameObject view)
    {
        if (!view)
        {
            Debug.Log("NO VIEW");
            return;
        }
        //Debug.Log(view);
        // 현재 _inGamePlayUI 비활성화
        //_inGamePlayUI.SetActive(false);

        // 목표 뷰 활성화
        view.SetActive(true);
    }

    public void HideViewAndReturn(GameObject view)
    {
        if (!view) return;

        // 현재 뷰 가리기
        view.SetActive(false);

        _inGamePlayUI.SetActive(true);
    }

    public void ShowMainGamePlayUI()
    {
        _inGamePlayUI.SetActive(true);
    }

    public void OnBackToPlayPressed(GameObject view)
    {
        HideViewAndReturn(view);
        Start_Timer();
    }

    protected virtual void OnMenuButtonPressed()
    {
        ShowView(MenuView);
        Stop_Tmer();
    }
    protected virtual void OnBackFromMenuView()
    {
        OnBackToPlayPressed(MenuView);
    }

    public void OnResultUI()
    {
        ShowView(ResultView);
        Stop_Tmer();
    }

    protected virtual void OnAcceptButtonPressed()
    {
        GameState.i.SaveGameData(_controlRateUI.slider.value, 10.0f);
        GameState.i.SetState(GAME.START);
    }
    public void OnGameOverUI()
    {
        ShowView(GameOverView);
        Stop_Tmer();
    }

    protected virtual void OnRestartButtonPressed()
    {
        GameState.i.SetState(GAME.START);
    }
    protected virtual void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenuScene_Name);
    }
}
