using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class TypeEventSystem
    {
        public static TypeEventSystem Global { get; } = new TypeEventSystem(); //默认全局类型事件系统

        private readonly Dictionary<Type, IEventItem> mTypeEvents = new Dictionary<Type, IEventItem>();

        public void Send<T>() where T : new()
        {
            GetEvent<EventItem<T>>()?.Trigger(new T());
        }

        public void Send<T>(T e)
        {
            GetEvent<EventItem<T>>()?.Trigger(e);
        }

        public IRegister Register<T>(Action<T> onEvent)
        {
            var e = GetOrAddEvent<EventItem<T>>();
            return e.Register(onEvent);
        }

        public void UnRegister<T>(Action<T> onEvent)
        {
            var e = GetEvent<EventItem<T>>();
            e?.UnRegister(onEvent);
        }

        private T AddEvent<T>() where T : IEventItem, new()
        {
            T t = new T();
            mTypeEvents.Add(typeof(T), t);
            return t;
        }

        private T GetEvent<T>() where T : IEventItem
        {
            if (mTypeEvents.TryGetValue(typeof(T), out IEventItem e))
            {
                return (T)e;
            }

            return default;
        }

        private T GetOrAddEvent<T>() where T : IEventItem, new()
        {
            Type eType = typeof(T);
            if (mTypeEvents.TryGetValue(eType, out IEventItem e))
            {
                return (T)e;
            }

            return AddEvent<T>();
        }
    }
}