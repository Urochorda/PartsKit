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
}