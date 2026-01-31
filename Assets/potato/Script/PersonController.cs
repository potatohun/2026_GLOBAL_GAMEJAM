using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class PersonController : MonoBehaviour
{
    public static PersonController instance;

    [Header("Settings")]
    [SerializeField] private int _showtime = 3;
    [SerializeField] private List<Sprite> _persons;

    [Header("Obejects")]
    [SerializeField] private GameObject _personObject;
    [SerializeField] private SpriteRenderer _personSprite;
    [SerializeField] private SpriteRenderer _textBackground;
    [SerializeField] private TMP_Text _personText;

    public ResultStateController _resultStateController;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Show()
    {
        // 초기화
        _personSprite.color = new Color(1, 1, 1, 0);
        _textBackground.color = new Color(1, 1, 1, 0);
        _personText.color = new Color(0, 0, 0, 1);
        _personText.text = "";

        // 랜덤 사람 가져오기
        _personSprite.sprite = _persons[Random.Range(0, _persons.Count)];

        string text = "";
        float similarity = _resultStateController.GetCurrentSimilarity();
        // 문장 선택
        switch (similarity)
        {
            case >= 70:
                text = "확실히 성공하고 오겠소이다.";
                break;
            case >= 60:
                text = "이정도면 충분하오.";
                break;
            case >= 50:
                text = "한 번 도전해보겠오.";
                break;
            case >= 30:
                text = "어쩔 수 없이 써야겠소...";
                break;
            case >= 10:
                text = "실패해도 내 탓은 하지 말길...";
                break;
            default:
                text = "장인이라해서 찾아왔건만...";
                break;
        }

        _personObject.SetActive(true);

        _personSprite.DOFade(1, 1);
        _textBackground.DOFade(1, 1).OnComplete(() =>
        {
            _personText.DOText(text, 1);
        });

        Invoke("Hide", _showtime);
    }

    public void Hide()
    {
        _personSprite.DOFade(0, 1);
        _personText.DOFade(0, 1);
        _textBackground.DOFade(0, 1).OnComplete(() =>
        {
            _personObject.SetActive(false);
            MaskCreateManager.instance.Next();
        });
    }
}
