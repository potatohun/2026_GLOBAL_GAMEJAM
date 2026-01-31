using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RequestPanelController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject _panel_request;
    [SerializeField] private Image _background;
    [SerializeField] private Image _baseMaskImage;
    [SerializeField] private Image _sussieImage;
    [SerializeField] private TMP_Text _requestText;
    [SerializeField] private RectTransform _requestImage;
    [SerializeField] private RectTransform _requestButtonArrow;

    public void Open()
    {
        _panel_request.SetActive(true);
        _background.DOFade(0.8f, 1f).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            _requestImage.DOAnchorPosY(0f, 1f).SetEase(Ease.InOutExpo);
            _sussieImage.DOFade(1f, 1f).SetEase(Ease.InOutExpo);
        });
    }

    public void Close()
    {
         _requestImage.DOAnchorPosY(1080f, 1f).SetEase(Ease.InOutExpo);
         _sussieImage.DOFade(0f, 1f).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            _background.DOFade(0f, 1f).SetEase(Ease.InOutExpo);
            _panel_request.SetActive(false);
            InGameUIController.instance.SetNextButton(true);
            MaskCreateManager.instance.Next();
        });
    }

    public void SetRequestImage(Sprite sprite)
    {
        _baseMaskImage.sprite = sprite;
    }
    
    public void OnClickRequestButton()
    {
        Close();
    }
}
