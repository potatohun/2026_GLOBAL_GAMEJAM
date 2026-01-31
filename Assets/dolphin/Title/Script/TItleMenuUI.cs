using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleMenuUI : MonoBehaviour
{
    public static TitleMenuUI i;

    // Views
    [Header("Views")]
    [SerializeField] private GameObject _mainMenuView;
    [SerializeField] private GameObject _OptionView;

    // Main Menu Buttons
    [Header("Main Menu UI")]
    [SerializeField] private Button _playButton;
    //[SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _optionButton;

    //Animator _controllerSelectViewAnim;
    private void Awake()
    {
        i = this;

        //_controllerSelectViewAnim = _controllerSelectView.GetComponent<Animator>();

        // 버튼 이벤트 연결
        if (_playButton)
            _playButton.onClick.AddListener(OnPlayButtonPressed);

        if (_exitButton)
            _exitButton.onClick.AddListener(OnQuitButtonPressed);

        if (_optionButton)
            _optionButton.onClick.AddListener(OnOptionButtonPressed);

    }

    public void ShowView(GameObject view)
    {
        if (!view)
        {
            Debug.Log("NO VIEW");
            return;
        }
        //Debug.Log(view);
        // 현재 MainMenuUI 비활성화
        _mainMenuView.SetActive(false);

        // 목표 뷰 활성화
        view.SetActive(true);
    }

    public void HideViewAndReturn(GameObject view)
    {
        if (!view) return;

        // 현재 뷰 가리기
        view.SetActive(false);

        // MainMenuUI 활성화

        _mainMenuView.SetActive(true);
    }

    public void ShowMainMenuView()
    {
        _mainMenuView.SetActive(true);
    }

    protected virtual void OnPlayButtonPressed()
    {
        GameState.i.SetState(GAME.START);
        //_controllerSelectViewAnim.Play("Show");
    }

    public void OnBackToMenuPressed(GameObject view)
    {
        HideViewAndReturn(view);
    }

    protected virtual void OnOptionButtonPressed()
    {
        ShowView(_OptionView);
    }
    protected virtual void OnBackFromOptionView()
    {
        OnBackToMenuPressed(_OptionView);
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
