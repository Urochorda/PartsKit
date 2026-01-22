using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PartsKit
{
    public class AnimEventTrigger : MonoBehaviour
    {
        public const string SendVoidName = "SendVoid";
        public const string SendFloatName = "SendFloat";
        public const string SendIntName = "SendInt";
        public const string SendStringName = "SendString";
        public const string SendObjectName = "SendObject";

        public event Action voidEvent;
        public event Action<float> floatEvent;
        public event Action<int> intEvent;
        public event Action<string> stringEvent;
        public event Action<Object> objectEvent;

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

        public void SendObject(Object value)
        {
            objectEvent?.Invoke(value);
        }
    }
}