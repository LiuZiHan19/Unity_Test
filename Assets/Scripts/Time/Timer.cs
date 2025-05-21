using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class Timer
{
    public bool IsCompleted => _duration <= 0;
    public bool IsPause { get; set; } = false;
    public float TotalDuration => _totalDuration;

    private float _totalDuration;
    private float _duration;

    private UnityAction _callback;
    private DateUnit _dateUnit;
    private StringBuilder _dateStringBuilder;

    private const float YearSeconds = 31536000;
    private const float MonthSeconds = 2592000;
    private const float DaySeconds = 86400;
    private const float HourSeconds = 3600;
    private const float MinuteSeconds = 60;

    public Timer(float duration, UnityAction callback)
    {
        _duration = duration;
        _totalDuration = duration;
        _callback = callback;
        _dateStringBuilder = new StringBuilder();
        SetDateUnit();
    }

    public void SetTimer(float duration, UnityAction callback)
    {
        _duration = duration;
        _totalDuration = duration;
        _callback = callback;
        IsPause = false;
    }

    public void SetDateUnit()
    {
        if (_dateUnit == null)
        {
            _dateUnit = new DateUnit("年", "月", "日", "时", "分", "秒", "yyyy年MM月dd日HH时mm分ss秒", "zh-CN");
        }
        else
        {
            _dateUnit.SetDateUnit("年", "月", "日", "时", "分", "秒", "yyyy年MM月dd日HH时mm分ss秒", "zh-CN");
        }
    }

    public void Update()
    {
        if (IsPause)
        {
            return;
        }

        _duration -= Time.deltaTime;

        if (IsCompleted)
        {
            _callback?.Invoke();
        }
    }

    public string CountdownDate()
    {
        _dateStringBuilder.Clear();
        float seconds = _duration;

        int year = (int)(seconds / YearSeconds);
        seconds %= YearSeconds;

        int month = (int)(seconds / MonthSeconds);
        seconds %= MonthSeconds;

        int day = (int)(seconds / DaySeconds);
        seconds %= DaySeconds;

        int hour = (int)(seconds / HourSeconds);
        seconds %= HourSeconds;

        int minute = (int)(seconds / MinuteSeconds);
        seconds %= MinuteSeconds;

        if (year > 0)
        {
            AppendUnit(year, _dateUnit.Year);
        }

        if (month > 0)
        {
            AppendUnit(month, _dateUnit.Month);
        }

        if (day > 0)
        {
            AppendUnit(day, _dateUnit.Day);
        }

        if (hour > 0)
        {
            AppendUnit(hour, _dateUnit.Hour);
        }

        if (minute > 0)
        {
            AppendUnit(minute, _dateUnit.Minute);
        }

        AppendUnit((int)seconds, _dateUnit.Second);

        return _dateStringBuilder.ToString();
    }

    public string ExpectedDate()
    {
        DateTime expectedTime = DateTime.Now.AddSeconds(TotalDuration);
        string expectedTimeString =
            expectedTime.ToString(_dateUnit.Format, CultureInfo.GetCultureInfo(_dateUnit.CultureInfo));
        return expectedTimeString;
    }

    private void AppendUnit(int value, string unit)
    {
        _dateStringBuilder.Append(value).Append(unit);
    }

    private class DateUnit
    {
        public string Format => _format;
        public string CultureInfo => _cultureInfo;

        private string _format;
        private string _cultureInfo;

        public DateUnit(string year, string month, string day, string hour, string minute, string second,
            string format, string cultureInfo)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            _format = format;
            _cultureInfo = cultureInfo;
        }

        public void SetDateUnit(string year, string month, string day, string hour, string minute, string second,
            string formatUnit, string cultureInfo)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            _format = formatUnit;
            _cultureInfo = cultureInfo;
        }

        public string Year;
        public string Month;
        public string Day;
        public string Hour;
        public string Minute;
        public string Second;
    }
}