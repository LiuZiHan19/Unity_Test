using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    public static TimeManager Instance => instance;

    private List<Timer> _timerList;
    private Queue<Timer> _timerToPool;
    private Queue<Timer> _timerPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _timerList = new List<Timer>();
        _timerToPool = new Queue<Timer>();
        _timerPool = new Queue<Timer>();
    }

    public Timer StartTimer(float duration, UnityAction callback)
    {
        Timer timer;
        if (_timerPool.Count > 0)
        {
            timer = _timerPool.Dequeue();
            timer.SetTimer(duration, callback);
        }
        else
        {
            timer = new Timer(duration, callback);
        }

        _timerList.Add(timer);

        return timer;
    }

    private void Update()
    {
        for (int i = 0; i < _timerList.Count; i++)
        {
            Timer timer = _timerList[i];
            timer.Update();

            if (timer.IsCompleted)
            {
                _timerToPool.Enqueue(timer);
            }
        }

        while (_timerToPool.Count > 0)
        {
            Timer timer = _timerToPool.Dequeue();
            _timerList.Remove(timer);
            _timerPool.Enqueue(timer);
        }
    }
}