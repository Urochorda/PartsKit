using UnityEngine;

namespace PartsKit
{
    public abstract class PartsKitBehaviour : MonoBehaviour
    {
        [SerializeField] protected bool autoInit;

        private bool isInit;

        private void Awake()
        {
            if (autoInit)
            {
                Init();
            }
        }

        private void OnDestroy()
        {
            DeInit();
        }

        public void Init()
        {
            if (isInit || !OnInitPre())
            {
                return;
            }

            isInit = true;
            OnInit();
        }

        public void DeInit()
        {
            if (!isInit)
            {
                return;
            }

            isInit = false;
            OnDeInit();
        }

        protected virtual bool OnInitPre()
        {
            return true;
        }

        protected abstract void OnInit();
        protected abstract void OnDeInit();
    }
}