using System.Collections.Generic;

namespace PartsKit
{
    public class LogicSequencePool
    {
        private readonly List<LogicSequence> pool = new List<LogicSequence>();
        private bool isClearing;

        public LogicSequence Get()
        {
            LogicSequence seq = LogicSequenceController.Instance.GetSequence();
            pool.Add(seq);
            seq.KillCallback += () =>
            {
                if (!isClearing)
                {
                    pool.RemoveDisorder(seq);
                }
            };
            return seq;
        }

        public void Clear()
        {
            isClearing = true;
            for (int i = 0, imax = pool.Count; i < imax; i++)
            {
                LogicSequence s = pool[i];
                if (s != null)
                {
                    s.Kill();
                }
            }

            pool.Clear();
            isClearing = true;
        }
    }
}