using UnityEngine;
using UnityEngine.UI;

public class ControlRateUI : MonoBehaviour
{
    public Slider slider;
    public float Max_Gage;
    public float Now_Gage;
    public float Limit_Gage;

    public float Reduce_Gage;

    public float ReduceTime;
    float timeLeft = 0;
    bool running = false;

    void OnEnable()
    {
        slider.minValue = Limit_Gage;
        slider.maxValue = Max_Gage;
        slider.value =Now_Gage;
    }

    public void SetGageData(float data)
    {
        Now_Gage = data;
        slider.value = Now_Gage;
    }

    void Update()
    {
        if (!running) return;

        timeLeft += Time.deltaTime;
        
        if(timeLeft >= ReduceTime)
        {
            Now_Gage -= Reduce_Gage;
            slider.value = Now_Gage;
            timeLeft = 0;
        }

        if (Now_Gage <= Limit_Gage)
        {
            running = false;
            OnTimeOver();
        }
    }

    void OnTimeOver()
    {
        GameState.i.SetState(GAME.END);
    }

    public void Pause() => running = false;
    public void Resume() => running = true;
}
