using UnityEngine;

namespace UniFramwork.Timer
{
    /// <summary>
    /// 倒计时结束后执行一次
    /// </summary>
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value)
        {
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
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}