using System.Threading;

namespace PartsKit
{
    public class RuntimeInsIdGenerator
    {
        private int nextId = 0;

        public RuntimeInsIdGenerator() : this(0)
        {
        }

        public RuntimeInsIdGenerator(int state)
        {
            SetState(state);
        }

        public int Generate()
        {
            return Interlocked.Increment(ref nextId);
        }

        public int GetState()
        {
            return nextId;
        }

        public void SetState(int state)
        {
            nextId = state;
        }
    }
}