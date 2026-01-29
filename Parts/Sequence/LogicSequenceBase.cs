using System;

namespace PartsKit
{
    public abstract class LogicSequenceBase
    {
        public bool IsValid { get; private set; }
        public bool IsPlaying { get; private set; } //是否正在播放，暂停：false
        public bool IsRunning { get; private set; } //是否正在运行，未播放或者已结束：false，暂停：true
        public event Action KillCallback;
        public bool IgnoreTimeScale { get; set; }

        public void Get()
        {
            //初始化
            IsValid = true;
            IsPlaying = false;
            IsRunning = false;
            KillCallback = null;
            IgnoreTimeScale = false;
            OnGet();
        }

        public void Play()
        {
            if (IsRunning)
            {
                return;
            }

            IsPlaying = true;
            IsRunning = true;
            OnPlay();
        }

        public void Kill()
        {
            if (!IsRunning)
            {
                return;
            }

            IsPlaying = false;
            IsRunning = false;
            IsValid = false;
            OnKill();
            KillCallback?.Invoke();
            KillCallback = null;
        }

        public void Reset()
        {
            Kill();
            OnReset();
        }

        public void SetPause(bool isPause)
        {
            if (!IsRunning)
            {
                return;
            }

            IsPlaying = !isPause;
            OnPause(isPause);
        }

        public bool IsPause()
        {
            return IsRunning && !IsPlaying;
        }

        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsPlaying)
            {
                return;
            }

            bool isComplete = OnUpdate(deltaTime, unscaledDeltaTime);

            //如果完成了则停止
            if (isComplete)
            {
                Kill();
            }
        }

        protected abstract void OnGet();
        protected abstract void OnPlay();
        protected abstract void OnKill();
        protected abstract void OnReset();
        protected abstract void OnPause(bool isPause);
        protected abstract bool OnUpdate(float deltaTime, float unscaledDeltaTime);
    }
}