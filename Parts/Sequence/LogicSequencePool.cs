using System.Collections.Generic;

namespace PartsKit
{
    public class LogicSequencePool
    {
        private readonly List<LogicSequence> mList = new List<LogicSequence>();

        public LogicSequence Get()
        {
            LogicSequence seq = LogicSequenceController.Instance.GetSequence();
            mList.Add(seq);
            return seq;
        }

        public void Clear()
        {
            for (int i = 0, imax = mList.Count; i < imax; i++)
            {
                LogicSequence s = mList[i];
                if (s != null)
                {
                    s.Kill();
                }
            }

            mList.Clear();
        }
    }
}