using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Image[] Success_Agent_UI;
    public Image[] Fail_Agent_UI;
    public Sprite[] Stamp;
    public Image StampImage;
    public float Bonus_Score;
    public TextMeshProUGUI AddScoreText;
    public Sprite blank;

    private void OnEnable()
    {
        Bonus_Score = 0;
        Dictionary<Sprite, float> SuccessAgentDic = GameManager.i.SuccessAgentDic;
        Dictionary<Sprite, float> FailAgentDic = GameManager.i.FailAgentDic;
        // GameManager.i.SuccessAgentDic.Clear();
        // GameManager.i.FailAgentDic.Clear();

        int SuccessMask = SuccessAgentDic.Count;

        if(SuccessMask >= 3)
        {
            Bonus_Score += 9;
        }
        else if(SuccessMask == 2)
        {
            Bonus_Score += 6;
        }
        else if(SuccessMask == 1)
        {
            Bonus_Score += 3;
        }

        int FailMask = FailAgentDic.Count;
        if (FailMask >= 3)
        {
            Bonus_Score -= 12;
        }
        else if (FailMask == 2)
        {
            Bonus_Score -= 6;
        }
        else if (FailMask == 1)
        {
            Bonus_Score -= 3;
        }

        SetStamp(Bonus_Score);
        SetAddScoreText(Bonus_Score);
        FindFiveSuccessAgent(SuccessAgentDic);
        FindFiveFailAgent(FailAgentDic);
    }

    private void SetStamp(float Bonus_Score)
    {
        if (Bonus_Score >= 6)
        {
            StampImage.sprite = Stamp[0];
        }
        else if (Bonus_Score >= 3)
        {
            StampImage.sprite = Stamp[1];
        }
        else if (Bonus_Score >= 1)
        {
            StampImage.sprite = Stamp[2];
        }
        else
        {
            StampImage.sprite = Stamp[3];
        }
    }

    private void SetAddScoreText(float Bonus_Score)
    {
        int bs = ((int)Bonus_Score);
        if (bs > 0)
        {
            AddScoreText.text = " 총 명성 " + "+" + bs;
        }
        else
        {
            AddScoreText.text = " 총 명성 " + bs;
        }
    }

    private void FindFiveSuccessAgent(Dictionary<Sprite, float> SD)
    {
        var list = new List<KeyValuePair<Sprite, float>>(SD);
        list.Sort((a,b) => b.Value.CompareTo(a.Value));

        if(list.Count < 5)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                Success_Agent_UI[i].sprite = list[i].Key;
            }
        }
        else
        {
            for(int i = 0; i < 5; ++i)
            {
                Success_Agent_UI[i].sprite = list[i].Key;
            }
        }

        int l_cnt = list.Count;
        while(l_cnt < 5)
        {
            Success_Agent_UI[l_cnt].sprite = blank;
            l_cnt++;
        }
    }

    private void FindFiveFailAgent(Dictionary<Sprite, float> FD)
    {
        var list = new List<KeyValuePair<Sprite, float>>(FD);
        list.Sort((a, b) => a.Value.CompareTo(b.Value));

        if (list.Count < 5)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Fail_Agent_UI[i].sprite = list[i].Key;
            }
        }
        else
        {
            for (int i = 0; i < 5; ++i)
            {
                Fail_Agent_UI[i].sprite = list[i].Key;
            }
        }

        int l_cnt = list.Count;
        while (l_cnt < 5)
        {
            Fail_Agent_UI[l_cnt].sprite = blank;
            l_cnt++;
        }
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
