using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI i;

    [SerializeField] private GameObject _inGamePlayUI;
    [SerializeField] private TimerUI _timerUI;
    [SerializeField] private ControlRateUI _controlRateUI;
    public GameObject ControlRateUI;
    public GameObject MenuView;
    ///public GameObject MenuButton;

    [SerializeField]
    private string MainMenuScene_Name;

    private void Awake()
    {
        i = this;
        MenuView.SetActive(false);
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
        _inGamePlayUI.SetActive(false);

        // 목표 뷰 활성화
        view.SetActive(true);
    }

    public void HideViewAndReturn(GameObject view)
    {
        if (!view) return;

        // 현재 뷰 가리기
        view.SetActive(false);

        // MainMenuUI 활성화

        _inGamePlayUI.SetActive(true);
    }

    public void ShowMainGamePlayUI()
    {
        _inGamePlayUI.SetActive(true);
    }

    public void OnBackToPlayPressed(GameObject view)
    {
        HideViewAndReturn(view);
    }

    protected virtual void OnMenuButtonPressed()
    {
        ShowView(MenuView);
    }
    protected virtual void OnBackFromMenuView()
    {
        OnBackToPlayPressed(MenuView);
    }

    protected virtual void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenuScene_Name);
    }
}
