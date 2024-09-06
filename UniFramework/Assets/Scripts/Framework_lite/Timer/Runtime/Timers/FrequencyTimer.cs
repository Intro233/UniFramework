using System;
using UnityEngine;

namespace UniFramwork.Timer
{
    /// <summary>
    /// 频率计时器，执行N次每秒
    /// </summary>
    public class FrequencyTimer : Timer
    {
        public int TicksPerSecond { get; private set; }

        public Action OnTick = delegate { };

        float timeThreshold;

        public FrequencyTimer(int ticksPerSecond) : base(0)
        {
            CalculateTimeThreshold(ticksPerSecond);
        }

        internal override void Tick()
        {
            if (IsPause || !IsRunning)
            {
                return;
            }
            if (CurrentTime >= timeThreshold)
            {
                Reset();
                OnTick?.Invoke();
            }
            else
            {
                CurrentTime += Time.deltaTime;
            }
        }

        public override bool IsFinished => !IsRunning;

        public override void Reset()
        {
            CurrentTime = 0;
        }

        public void Reset(int newTicksPerSecond)
        {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        void CalculateTimeThreshold(int ticksPerSecond)
        {
            TicksPerSecond = ticksPerSecond;
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}