using System;

namespace PartsKit
{
    public class LogicSequenceCallback : LogicSequenceBase
    {
        private readonly Action mCallback;

        public LogicSequenceCallback(Action callback)
        {
            mCallback = callback;
        }

        protected override void OnGet()
        {
        }

        protected override void OnPlay()
        {
        }

        protected override void OnKill()
        {
        }

        protected override void OnReset()
        {
        }

        protected override void OnPause(bool isPause)
        {
        }

        protected override bool OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            mCallback?.Invoke();
            return true;
        }
    }
}