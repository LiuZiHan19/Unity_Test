using System;
using UnityEngine;
using UnityEngine.Events;

public class Timer
{
    public bool IsCompleted => _time <= 0;

    public bool IsPause { get; set; } = false;

    private float _time;

    private UnityAction _callback;

    public Timer(float time, UnityAction callback)
    {
        _time = time;
        _callback = callback;
    }

    public void Initialise(float time, UnityAction callback)
    {
        _time = time;
        _callback = callback;
    }

    public void Update()
    {
        if (IsPause)
        {
            return;
        }

        _time -= Time.deltaTime;

        if (IsCompleted)
        {
            _callback?.Invoke();
        }
    }

    public string GetTimeText()
    {
        // todo : convert to string
        TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.Max(0, _time));
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}