using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    [SerializeField] private Text timeText;
    private Timer _timer;

    private void Start()
    {
        _timer = TimeManager.Instance.StartTimer(100000, OnCompleted);
    }

    private void Update()
    {
        if (_timer.IsCompleted == false)
        {
            timeText.text = _timer.GetTimeText();
        }
    }

    public void OnCompleted()
    {
        timeText.text = "完成";
    }
}