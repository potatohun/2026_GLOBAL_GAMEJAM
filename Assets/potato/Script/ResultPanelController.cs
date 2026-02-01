using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ResultPanelController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject _panel_result;
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _resultRectTransform;
    [SerializeField] private Image _baseMaskImage;
    [SerializeField] private Image _resultMaskImage;
    [SerializeField] private TMP_Text _similarityText;

    public ResultStateController _resultStateController;

    public void Open()
    {
        AudioManager.instance.PlayOptionSound();
        
        _panel_result.SetActive(true);
        _background.DOFade(0.8f, 1f).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            _resultRectTransform.DOAnchorPosY(0f, 1f).SetEase(Ease.InOutExpo);
        });
    }

    public void Close()
    {
        AudioManager.instance.PlayCloseOptionSound();

        _resultRectTransform.DOAnchorPosY(1080f, 1f).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            _background.DOFade(0f, 1f).SetEase(Ease.InOutExpo);
            _panel_result.SetActive(false);
        });
    }

    public void SetResultImage(Sprite baseSprite, Sprite resultSprite)
    {
        _baseMaskImage.sprite = baseSprite;
        _resultMaskImage.sprite = resultSprite;
    }
    
    public void SetSimilarity(float similarity)
    {
        _similarityText.text = "완성도 : " + similarity.ToString("F2") + "%";
    }

    public void OnSellButtonClick()
    {
        AudioManager.instance.PlayClickSound();

        // TO DO : 점수 보냄
        GameManager.i.SentAgent(_resultStateController.GetCurrentResultSprite(), _resultStateController.GetCurrentSimilarity());

        PersonController.instance.Show();

        Close();
    }

    public void OnDepositButtonClick()
    {
         AudioManager.instance.PlayClickSound();

        // 다음으로 넘어가기
        MaskCreateManager.instance.Next();
        
        Close();
    }
}
