using System;
using UnityEngine;

namespace UniFramwork.Timer
{
    public abstract class Timer : IDisposable
    {
        /// <summary>
        /// 初始化的倒计时
        /// </summary>
        protected float initialTime;

        /// <summary>
        /// 当前的倒计时
        /// </summary>
        public float CurrentTime { get; protected set; }

        /// <summary>
        /// 定时器是否开启
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPause { get; private set; }

        /// <summary>
        /// 定时器进度
        /// </summary>
        public float Progress => Mathf.Clamp(CurrentTime / initialTime, 0, 1);

        /// <summary>
        /// 是否结束
        /// </summary>
        public virtual bool IsFinished => !IsRunning;

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };
        public Action OnTimerCancel = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void Start()
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                IsPause = false;
                TimerManager.RegisterTimer(this);
                OnTimerStart?.Invoke();
            }
        }

        /// <summary>
        /// 定时器更新，请在子类实现此方法具体逻辑
        /// </summary>
        internal abstract void Tick();

        /// <summary>
        /// 自然停止定时器
        /// </summary>
        protected void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                IsPause = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop?.Invoke();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (IsRunning)
            {
                IsPause = true;
            }
        }

        /// <summary>
        /// 从暂停中恢复
        /// </summary>
        public void Resume()
        {
            if (IsRunning)
            {
                IsPause = false;
            }
        }

        /// <summary>
        /// 取消定时器
        /// </summary>
        public void Cancel()
        {
            if (IsRunning)
            {
                IsRunning = false;
                IsPause = false;
                TimerManager.DeregisterTimer(this);
                OnTimerCancel?.Invoke();
            }
        }

        /// <summary>
        /// 计时重置
        /// </summary>
        public virtual void Reset()
        {
            CurrentTime = initialTime;
        }

        /// <summary>
        /// 计时重置
        /// </summary>
        /// <param name="newTime">重新设置的倒计时时长</param>
        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        bool disposed;

        ~Timer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }

            disposed = true;
        }
    }
}