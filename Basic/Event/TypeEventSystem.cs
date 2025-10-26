using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class TypeEventSystem
    {
        private class TypeEventItem<T> : IEventItem
        {
            private Action<T> mOnEvent;
            private readonly Action<TypeEventItem<T>, Action<T>> mOnInteriorUnRegister;

            public TypeEventItem(Action<TypeEventItem<T>, Action<T>> onInteriorUnRegister)
            {
                mOnInteriorUnRegister = onInteriorUnRegister;
            }

            public IRegister Register(Action<T> onEvent)
            {
                mOnEvent += onEvent;
                return new ActionRegister(() => { mOnInteriorUnRegister?.Invoke(this, onEvent); });
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

        public static TypeEventSystem Global { get; } = new TypeEventSystem(); //默认全局类型事件系统

        private readonly Dictionary<Type, IEventItem> mTypeEvents = new Dictionary<Type, IEventItem>();

        public void Send<T>() where T : new()
        {
            GetEvent<T>()?.Trigger(new T());
        }

        public void Send<T>(T e)
        {
            GetEvent<T>()?.Trigger(e);
        }

        public IRegister Register<T>(Action<T> onEvent)
        {
            var e = GetOrAddEvent<T>();
            return e.Register(onEvent);
        }

        public void UnRegister<T>(Action<T> onEvent)
        {
            var e = GetEvent<T>();
            DoUnRegister(e, onEvent);
        }

        public void UnAllRegister()
        {
            foreach (var eventItem in mTypeEvents)
            {
                eventItem.Value.UnAllRegister();
            }

            mTypeEvents.Clear();
        }

        private void DoUnRegister<T>(TypeEventItem<T> eventItem, Action<T> onEvent)
        {
            eventItem?.UnRegister(onEvent);
        }

        private TypeEventItem<T> AddEvent<T>()
        {
            TypeEventItem<T> t = new TypeEventItem<T>(DoUnRegister);
            mTypeEvents.Add(typeof(T), t);
            return t;
        }

        private TypeEventItem<T> GetEvent<T>()
        {
            if (mTypeEvents.TryGetValue(typeof(T), out IEventItem e))
            {
                return e as TypeEventItem<T>;
            }

            return null;
        }

        private TypeEventItem<T> GetOrAddEvent<T>()
        {
            TypeEventItem<T> e = GetEvent<T>() ?? AddEvent<T>();
            return e;
        }
    }
}