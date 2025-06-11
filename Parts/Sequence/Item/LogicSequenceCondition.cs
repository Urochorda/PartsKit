using System;

namespace PartsKit
{
    public class LogicSequenceCondition : LogicSequenceBase
    {
        private Func<bool> mCondition;

        public LogicSequenceCondition(Func<bool> condition)
        {
            mCondition = condition;
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
            if (mCondition == null)
            {
                return true;
            }

            return mCondition.Invoke();
        }
    }
}