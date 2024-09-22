using System;

namespace PartsKit
{
    public class BoolCounter
    {
        private int selfCount;

        public int Count
        {
            get => selfCount;
            set
            {
                selfCount = value;
                UpdateCount();
            }
        }

        public bool Value { get; private set; }
        public event Action<bool, BoolCounter> onChange;

        public BoolCounter(int initCount)
        {
            Count = initCount;
        }

        public void Trigger()
        {
            onChange?.Invoke(Value, this);
        }

        private void UpdateCount()
        {
            bool oldValue = Value;
            Value = Count > 0;
            if (oldValue != Value)
            {
                onChange?.Invoke(oldValue, this);
            }
        }
    }
}