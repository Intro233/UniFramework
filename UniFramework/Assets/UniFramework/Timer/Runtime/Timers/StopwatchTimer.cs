using UnityEngine;

namespace UniFramwork.Timer
{
    /// <summary>
    /// 测量定时器开启之后持续的时间
    /// </summary>
    public class StopwatchTimer : Timer
    {
        public float CurrentUnScaledTime { get; private set; }

        public StopwatchTimer() : base(0)
        {
        }

        internal override void Tick()
        {
            if (IsPause || !IsRunning)
            {
                return;
            }
            CurrentTime += Time.deltaTime;
            CurrentUnScaledTime += Time.unscaledDeltaTime;
        }

        public override bool IsFinished => false;
    }
}