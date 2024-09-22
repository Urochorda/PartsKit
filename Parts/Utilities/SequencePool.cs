using System.Collections.Generic;
using DG.Tweening;

namespace PartsKit
{
    public class SequencePool
    {
        private readonly List<Sequence> pool = new List<Sequence>();

        private bool isClearing;

        public Sequence Get()
        {
            Sequence seq = DOTween.Sequence();
            pool.Add(seq);
            seq.OnKill(() =>
            {
                if (!isClearing)
                {
                    pool.RemoveDisorder(seq);
                }
            });
            return seq;
        }

        public void Clear(bool complete = false)
        {
            isClearing = true;
            for (int i = 0, imax = pool.Count; i < imax; i++)
            {
                Sequence s = pool[i];
                if (s != null)
                {
                    s.Kill(complete);
                }
            }

            pool.Clear();
            isClearing = false;
        }
    }
}