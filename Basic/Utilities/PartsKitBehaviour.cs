using UnityEngine;

namespace PartsKit
{
    public abstract class PartsKitBehaviour : MonoBehaviour
    {
        public bool IsInit { get; private set; }

        public void Init()
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
            OnInit();
        }

        public void DeInit()
        {
            if (!IsInit)
            {
                return;
            }

            IsInit = false;
            OnDeInit();
        }

        protected abstract void OnInit();
        protected abstract void OnDeInit();
    }

    public abstract class PartsKitBehaviour<T> : MonoBehaviour
    {
        public bool IsInit { get; private set; }

        public void Init(T t)
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
            OnInit(t);
        }

        public void DeInit()
        {
            if (!IsInit)
            {
                return;
            }

            IsInit = false;
            OnDeInit();
        }

        protected abstract void OnInit(T t);
        protected abstract void OnDeInit();
    }

    public abstract class PartsKitBehaviour<T, T2> : MonoBehaviour
    {
        public bool IsInit { get; private set; }

        public void Init(T t, T2 t2)
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
            OnInit(t, t2);
        }

        public void DeInit()
        {
            if (!IsInit)
            {
                return;
            }

            IsInit = false;
            OnDeInit();
        }

        protected abstract void OnInit(T t, T2 t2);
        protected abstract void OnDeInit();
    }

    public abstract class PartsKitBehaviour<T, T2, T3> : MonoBehaviour
    {
        public bool IsInit { get; private set; }

        public void Init(T t, T2 t2, T3 t3)
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
            OnInit(t, t2, t3);
        }

        public void DeInit()
        {
            if (!IsInit)
            {
                return;
            }

            IsInit = false;
            OnDeInit();
        }

        protected abstract void OnInit(T t, T2 t2, T3 t3);
        protected abstract void OnDeInit();
    }
}