using System;
using UnityEngine;

namespace PartsKit
{
    public class AnimEvent : MonoBehaviour
    {
        public event Action voidEvent;
        public event Action<float> floatEvent;
        public event Action<int> intEvent;
        public event Action<string> stringEvent;

        public void SendVoid()
        {
            voidEvent?.Invoke();
        }

        public void SendFloat(float value)
        {
            floatEvent?.Invoke(value);
        }

        public void SendInt(int value)
        {
            intEvent?.Invoke(value);
        }

        public void SendString(string value)
        {
            stringEvent?.Invoke(value);
        }
    }
}