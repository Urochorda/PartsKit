using System;
using UnityEngine;

namespace PartsKit
{
    public class UIPanel : MonoBehaviour
    {
        public bool IsOpen { get; private set; }
        public string PanelKey { get; private set; }
        public Action OnCloseCall { get; set; }
        private UIController thisController;

        /// <summary>
        /// 设置创建，由Controller调用，不要调用
        /// </summary>
        /// <param name="uiPanel"></param>
        public static void SetCreate(UIPanel uiPanel)
        {
            uiPanel.OnCreate();
        }

        /// <summary>
        /// 设置打开，由Controller调用，不要调用
        /// </summary>
        public static void SetOpen(UIPanel uiPanel, UIController controller, string panelKey)
        {
            uiPanel.IsOpen = true;
            uiPanel.thisController = controller;
            uiPanel.PanelKey = panelKey;
            uiPanel.OnOpen();
        }

        /// <summary>
        /// 设置关闭，由Controller调用，不要调用
        /// </summary>
        public static void SetClose(UIPanel uiPanel)
        {
            uiPanel.IsOpen = false;
            uiPanel.thisController = null;
            uiPanel.PanelKey = string.Empty;
            uiPanel.OnClose();
            uiPanel.OnCloseCall?.Invoke();
            uiPanel.OnCloseCall = null;
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="isDestroy">是否销毁页面</param>
        public void Close(bool isDestroy)
        {
            thisController.ClosePanel(PanelKey, isDestroy);
        }

        /// <summary>
        /// 创建后即可调用，不管isActive或者isEnable，只有创建时调用一次
        /// </summary>
        protected virtual void OnCreate()
        {
        }

        /// <summary>
        /// 页面打开时调用，重复打开关闭可能多次调用
        /// </summary>
        protected virtual void OnOpen()
        {
        }

        /// <summary>
        /// 页面关闭时调用，重复打开关闭可能多次调用
        /// </summary>
        protected virtual void OnClose()
        {
        }
    }
}