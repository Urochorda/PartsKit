using System;

namespace PartsKit
{
    public class EventItem : IEventItem
    {
        private Action mOnEvent;

        public IRegister Register(Action onEvent)
        {
            mOnEvent += onEvent;
            return new ActionRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger()
        {
            mOnEvent?.Invoke();
        }

        public void UnAllRegister()
        {
            mOnEvent = null;
        }
    }

    public class EventItem<T> : IEventItem
    {
        private Action<T> mOnEvent;

        public IRegister Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
            return new ActionRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger(T t)
        {
            mOnEvent?.Invoke(t);
        }

        public void UnAllRegister()
        {
            mOnEvent = null;
        }
    }

    public class EventItem<T, K> : IEventItem
    {
        private Action<T, K> mOnEvent;

        public IRegister Register(Action<T, K> onEvent)
        {
            mOnEvent += onEvent;
            return new ActionRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, K> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger(T t, K k)
        {
            mOnEvent?.Invoke(t, k);
        }

        public void UnAllRegister()
        {
            mOnEvent = null;
        }
    }

    public class EventItem<T, K, S> : IEventItem
    {
        private Action<T, K, S> mOnEvent;

        public IRegister Register(Action<T, K, S> onEvent)
        {
            mOnEvent += onEvent;
            return new ActionRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, K, S> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger(T t, K k, S s)
        {
            mOnEvent?.Invoke(t, k, s);
        }

        public void UnAllRegister()
        {
            mOnEvent = null;
        }
    }
}