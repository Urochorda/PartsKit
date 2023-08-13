using System;

namespace PartsKit
{
    /// <summary>
    /// 自定义可注销的类
    /// </summary>
    public struct ActionRegister : IRegister
    {
        /// <summary>
        /// 委托对象
        /// </summary>
        private Action mOnUnRegister { get; set; }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="onUnRegister"></param>
        public ActionRegister(Action onUnRegister)
        {
            mOnUnRegister = onUnRegister;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void UnRegister()
        {
            mOnUnRegister?.Invoke();
            mOnUnRegister = null;
        }
    }
}
