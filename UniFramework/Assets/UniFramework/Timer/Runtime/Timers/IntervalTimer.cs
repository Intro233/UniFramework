using System;
using UniFramwork.Timer;
using UnityEngine;

namespace UniFramwork.Timer
{
    /// <summary>
    /// 间隔N秒触发一次
    /// </summary>
    public class IntervalTimer : Timer
    {
        public Action OnTick = delegate { };

        private float mIntervalTime;

        public IntervalTimer(float intervalTime) : base(intervalTime)
        {
            mIntervalTime = intervalTime;
        }

        internal override void Tick()
        {
            if (IsPause || !IsRunning)
            {
                return;
            }

            if (CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }
            else
            {
                OnTick?.Invoke();
                Reset();
            }
        }

        public override bool IsFinished => false;
    }
}