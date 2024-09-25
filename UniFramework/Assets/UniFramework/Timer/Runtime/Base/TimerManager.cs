using System.Collections.Generic;

namespace UniFramwork.Timer
{
    public static class TimerManager
    {
        static readonly List<Timer> timers = new(16);
        static readonly List<Timer> sweep = new(16);

        public static void RegisterTimer(Timer timer)
        {
            timers.Add(timer);
        }


        public static void DeregisterTimer(Timer timer)
        {
            timers.Remove(timer);
        }

        public static void UpdateTimers()
        {
            if (timers.Count == 0) return;
            sweep.RefreshWith(timers);
            foreach (var timer in sweep)
            {
                timer.Tick();
            }
        }

        public static void Clear()
        {
            sweep.RefreshWith(timers);
            foreach (var timer in sweep)
            {
                timer.Dispose();
            }

            timers.Clear();
            sweep.Clear();
        }
    }
}