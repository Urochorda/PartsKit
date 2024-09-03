using System.Collections.Generic;
using DG.Tweening;

namespace PartsKit
{
    public class SequencePool
    {
        private readonly List<Sequence> pool = new List<Sequence>();

        public Sequence Get()
        {
            Sequence seq = DOTween.Sequence();
            pool.Add(seq);
            return seq;
        }

        public void Clear(bool complete = false)
        {
            for (int i = 0, imax = pool.Count; i < imax; i++)
            {
                Sequence s = pool[i];
                if (s != null)
                {
                    s.Kill(complete);
                }
            }

            pool.Clear();
        }
    }
}