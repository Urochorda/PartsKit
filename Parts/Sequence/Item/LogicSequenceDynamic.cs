using System;

namespace PartsKit
{
    public class LogicSequenceDynamic : LogicSequenceBase
    {
        private LogicSequenceBase sequence;
        private readonly Func<LogicSequenceBase> mGetSequence;

        public LogicSequenceDynamic(Func<LogicSequenceBase> getSequence)
        {
            mGetSequence = getSequence;
        }

        protected override void OnGet()
        {
        }

        protected override void OnPlay()
        {
            sequence = mGetSequence?.Invoke();
            if (sequence != null)
            {
                sequence.Play();
            }
        }

        protected override void OnKill()
        {
            if (sequence != null)
            {
                sequence.Kill();
            }
        }

        protected override void OnReset()
        {
            if (sequence != null)
            {
                sequence.Reset();
            }
        }

        protected override void OnPause(bool isPause)
        {
            if (sequence != null)
            {
                sequence.SetPause(isPause);
            }
        }

        protected override bool OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (sequence == null)
            {
                return true;
            }

            sequence.Update(deltaTime, unscaledDeltaTime);
            return !sequence.IsRunning;
        }
    }
}