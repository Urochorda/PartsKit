using UnityEngine;

namespace PartsKit
{
    public class UIPanel : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        /// <summary>
        /// 设置打开，由Controller调用，不要调用
        /// </summary>
        public void SetOpen()
        {
            IsOpen = true;
            OnOpen();
        }

        /// <summary>
        /// 设置关闭，由Controller调用，不要调用
        /// </summary>
        public void SetClose()
        {
            IsOpen = false;
            OnClose();
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }
    }

    public class UIPanel<T> : UIPanel
    {
        protected T Data { get; private set; }

        public void SetData(T data)
        {
            Data = data;
        }
    }
}