using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        AudioManager.instance.PlayInGameBGM();
        HideViewAndReturn(view);
        Start_Timer();
    }

    protected virtual void OnMenuButtonPressed()
    {
        AudioManager.instance.PauseBGM();
        AudioManager.instance.PlayOptionSound();
        ShowView(MenuView);
        Stop_Tmer();
    }
    protected virtual void OnBackFromMenuView()
    {
        AudioManager.instance.PlayCloseOptionSound();
        OnBackToPlayPressed(MenuView);
    }

    public void OnResultUI()
    {
        AudioManager.instance.PlayResultSound();
        ShowView(ResultView);
        Stop_Tmer();
    }

    protected virtual void OnAcceptButtonPressed()
    {
        AudioManager.instance.PlayClickSound();
        AudioManager.instance.StopBGM();
        GameState.i.SaveGameData(_controlRateUI.slider.value, _resultUI.Bonus_Score);
        GameState.i.SetState(GAME.START);
    }
    public void OnGameOverUI()
    {
        AudioManager.instance.PlayResultSound();
        ShowView(GameOverView);
        Stop_Tmer();
    }

    protected virtual void OnRestartButtonPressed()
    {
        AudioManager.instance.PlayClickSound();
        GameState.i.SetState(GAME.START);
    }
    protected virtual void GoToMainMenu()
    {
        AudioManager.instance.PlayClickSound();
        AudioManager.instance.StopBGM();
        SceneManager.LoadScene(MainMenuScene_Name);
    }

    public void UpdateConquer(float data)
    {
        _controlRateUI.slider.value += data;
    }
}
