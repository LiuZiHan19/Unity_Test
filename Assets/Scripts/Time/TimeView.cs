using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    [SerializeField] private Text timeText;
    [SerializeField] private Text expectedTimeText;
    [SerializeField] private float time;
    private Timer _timer;

    private void Start()
    {
        _timer = TimeManager.Instance.StartTimer(time, OnCompleted);
        expectedTimeText.text = _timer.ExpectedDate();
    }

    private void Update()
    {
        if (_timer.IsCompleted == false)
        {
            timeText.text = _timer.CountdownDate();
        }
    }

    public void OnCompleted()
    {
        timeText.text = "完成";
    }
}