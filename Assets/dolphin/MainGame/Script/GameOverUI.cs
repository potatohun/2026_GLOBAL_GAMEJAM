using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI Tal_num;
    public TextMeshProUGUI Score_num;

    private void OnEnable()
    {
        Tal_num.text = GameManager.i.Tal_cnt.ToString();
        Score_num.text = GameManager.i.Max_Score.ToString();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
