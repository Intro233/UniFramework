using System;
using UniFramwork.Timer;
using UnityEngine;

public enum TimerType
{
    CountDown,
    Frequency,
    Interval,
    StopWatch,
}

public class TestTimer : MonoBehaviour
{
    public TimerType timerType;
    [Range(0, 1)] public float timeScale;
    private Timer timer;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Time.timeScale = timeScale;
        switch (timerType)
        {
            case TimerType.CountDown:
                timer = new CountdownTimer(5f);
                if (timer is CountdownTimer countdownTimer)
                {
                    countdownTimer.OnTimerStop += () => { Debug.Log("Down!"); };
                }

                break;
            case TimerType.Frequency:
                timer = new FrequencyTimer(2);
                if (timer is FrequencyTimer frequencyTimer)
                {
                    frequencyTimer.OnTick += () => { Debug.Log("Tick"); };
                }

                break;
            case TimerType.Interval:
                timer = new IntervalTimer(2);
                if (timer is IntervalTimer intervalTimer)
                {
                    intervalTimer.OnTick += () => { Debug.Log("Tick"); };
                }

                break;
            case TimerType.StopWatch:
                timer = new StopwatchTimer();
                if (timer is StopwatchTimer stopwatchTimer)
                {
                    stopwatchTimer.OnTimerCancel += () =>
                    {
                        Debug.Log($"STOPWATCH:\n" +
                                  $"Scaled:{stopwatchTimer.CurrentTime}\n" +
                                  $"UnScaled:{stopwatchTimer.CurrentUnScaledTime}");
                    };
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            timer.Start();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            timer.Pause();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            timer.Resume();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            timer.Reset();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            timer.Cancel();
        }

        if (timer.IsRunning)
        {
            if (timer is StopwatchTimer || timer is CountdownTimer)
            {
                Debug.Log($"进度：{timer.Progress};当前{timer.CurrentTime}");
            }
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 100;
        GUILayout.Label("S:开启定时器\nP：暂停\nL：恢复暂停\nR：重置\nD:取消", style);
    }

    private void OnValidate()
    {
        Init();
    }
}