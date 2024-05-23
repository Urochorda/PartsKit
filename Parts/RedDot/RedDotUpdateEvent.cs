using System.Collections.Generic;

namespace PartsKit
{
    public class RedDotUpdateEvent
    {
        public List<RedDot> RedDot { get; }

        public RedDotUpdateEvent()
        {
            RedDot = new List<RedDot>();
        }
    }
}