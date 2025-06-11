namespace PartsKit
{
    public class LogicSequenceInterval : LogicSequenceBase
    {
        public float Duration { get; }

        private float curDuration;

        public LogicSequenceInterval(float duration)
        {
            Duration = duration;
            curDuration = 0;
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
            curDuration = 0;
        }

        protected override void OnPause(bool isPause)
        {
        }

        protected override bool OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            curDuration += IgnoreTimeScale ? unscaledDeltaTime : deltaTime;
            return curDuration >= Duration;
        }
    }
}