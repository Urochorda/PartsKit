using System.Collections.Generic;
using System.Linq;

namespace PartsKit
{
    public class LogicSequenceJoin : LogicSequenceBase
    {
        private readonly List<LogicSequenceBase> sequencePool = new List<LogicSequenceBase>();

        protected override void OnGet()
        {
            sequencePool.Clear();
        }

        protected override void OnPlay()
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Play();
            }
        }

        protected override void OnKill()
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Kill();
            }
        }

        protected override void OnReset()
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Reset();
            }

            sequencePool.Clear();
        }

        protected override void OnPause(bool isPause)
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.SetPause(isPause);
            }
        }

        protected override bool OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Update(deltaTime, unscaledDeltaTime);
            }

            //全部为空或者不处于运行状态就算完成
            return sequencePool.All(item => item == null || !item.IsRunning);
        }

        public void Add(LogicSequenceBase sequence)
        {
            if (sequence == null)
            {
                return;
            }

            sequencePool.Add(sequence);
        }
    }
}