using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameUIController : MonoBehaviour
{
    public static InGameUIController instance;

    [Header("Buttons")]
    [SerializeField] private Button _nextButton; // 다음 단계 이동
    [SerializeField] private Button _previewButton; // 미리보기
    [SerializeField] private float _nextButtonCooldown = 2f;
    private float _nextButtonLastCallTime = -999f;

    [Header("Preview Panels")]
    [SerializeField] private RectTransform _previewPanel; // 미리보기 패널
    [SerializeField] private Image _previewImage; // 미리보기 이미지

    [SerializeField] private Image _previewButtonArrow;
    [SerializeField] private float _previewAutoCloseDelay = 3f;

    private bool _isOpenPreviewPanel = false;

    [Header("Result Panels")]
    [SerializeField] private RectTransform _resultPanel; // 결과 패널
    [SerializeField] private Image _resultBackgroundPanel; // 결과 배경 패널
    [SerializeField] private RectTransform _resultContentPanel; // 결과 내용 패널
    private bool _isOpenResultPanel = false;
    
    [SerializeField] private Ease _easeType; // 여닫기 이펙트 타입

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
            
        SetResultPanel(false);
    }
    public void OnClickNextButton()
    {
        if (Time.time - _nextButtonLastCallTime < _nextButtonCooldown)
            return;

        _nextButtonLastCallTime = Time.time;

        MaskCreateManager.instance.Next();
    }

    public void SetActivePreviewPanel(bool isActive)
    {
        _previewPanel.gameObject.SetActive(isActive);
    }

    public void OnClickPreviewButton()
    {
        if (_isOpenPreviewPanel)
        {
            // 미리보기 닫기
            _previewPanel.DOAnchorPosX(-660f, 0.5f).SetEase(_easeType).OnComplete(() =>
            {
                _isOpenPreviewPanel = false;
                _previewButtonArrow.transform.localScale = new Vector3(-1, 1, 1);
            });
        }
        else
        {
            // 미리보기 열기
            _previewPanel.DOAnchorPosX(-60f, 0.5f).SetEase(_easeType).OnComplete(() =>
            {
                _isOpenPreviewPanel = true;
                _previewButtonArrow.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }

    public void SetPreviewPanel(bool isOpen)
    {
        if (isOpen)
        {
            _previewPanel.DOKill(true);
            _previewPanel.DOAnchorPosX(-60f, 0.5f).SetEase(_easeType).OnComplete(() =>
            {
                _isOpenPreviewPanel = true;
                _previewButtonArrow.transform.localScale = new Vector3(1, 1, 1);
            });
        }
        else
        {
            _previewPanel.DOKill(true);
            _previewPanel.DOAnchorPosX(-660f, 0.5f).SetEase(_easeType).OnComplete(() =>
            {
                _isOpenPreviewPanel = false;
                _previewButtonArrow.transform.localScale = new Vector3(-1, 1, 1);
            });
        }
    }

    public void SetPreviewImage(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("Preview Image is null.");
            _previewImage.sprite = null;
        }

        Debug.Log("Set Preview Image: " + sprite.name);
        _previewImage.sprite = sprite;
    }

    public void SetNextButton(bool isActive)
    {
        if (isActive)
        {
            _nextButton.gameObject.SetActive(true);
            _nextButton.GetComponent<Image>().DOFade(1f, 1f).SetEase(_easeType).OnComplete(() =>
            {
                _nextButton.interactable = true;
            });
        }
        else
        {
            _nextButton.interactable = false;
            _nextButton.GetComponent<Image>().DOFade(0f, 1f).SetEase(_easeType).OnComplete(() =>
            {
                _nextButton.gameObject.SetActive(false);
                
            });
        }
    }

    public void SetResultPanel(bool isOpen)
    {
        if (isOpen)
        {
            _resultPanel.gameObject.SetActive(true);
            _resultBackgroundPanel.DOFade(0.8f, 1f).SetEase(_easeType);
            _resultContentPanel.DOAnchorPosY(0f, 1f).SetEase(_easeType);
        }
        else
        {
            _resultBackgroundPanel.DOFade(0f, 1f).SetEase(_easeType);
            _resultContentPanel.DOAnchorPosY(-this._resultContentPanel.rect.height, 1f).SetEase(_easeType).OnComplete(() =>
            {
                _resultPanel.gameObject.SetActive(false);
            });
        }
    }

    public void OnClickSellButton()
    {
        Debug.Log("Set Sell Button");
        MaskCreateManager.instance.Next();
        SetResultPanel(false);
    }

    public void OnClickDisposeButton()
    {
        Debug.Log("Set Dispose Button");
        MaskCreateManager.instance.Next();
        SetResultPanel(false);
    }
}
