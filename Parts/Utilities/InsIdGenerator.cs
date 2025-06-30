using System.Threading;

namespace PartsKit
{
    public static class InsIdGenerator
    {
        private static int nextId = 0;

        public static int Generate()
        {
            return Interlocked.Increment(ref nextId);
        }
    }
}