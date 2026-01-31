using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public RectTransform needle;
    public float limitTime;

    private float elapsed;

    bool running = false;

    void OnEnable()
    {
        elapsed = 0f;
        needle.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (!running) return;

        if (elapsed >= limitTime) return;

        elapsed += Time.deltaTime;
        float t = elapsed / limitTime;

        // 0 → -360도 (시계 방향)
        float angle = Mathf.Lerp(0f, -360f, t);
        needle.localRotation = Quaternion.Euler(0f, 0f, angle);

        if(elapsed == 60.0f)
        {
            AudioManager.instance.ReportSound();
        }

        if (elapsed >= limitTime)
        {
            running = false;
            OnTimeOver();
        }
    }
    void OnTimeOver()
    {
        GameState.i.SetState(GAME.RESULT);
    }
    public void Pause() => running = false;
    public void Resume() => running = true;
}
