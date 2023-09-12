using UnityEngine;

namespace PartsKit
{
    public class UIPanel : MonoBehaviour
    {
        public bool IsOpen { get; private set; }
        public string PanelKey { get; private set; }
        private UIPanelController thisPanelController;

        /// <summary>
        /// 设置打开，由Controller调用，不要调用
        /// </summary>
        public static void SetOpen(UIPanel uiPanel, UIPanelController panelController, string panelKey)
        {
            uiPanel.IsOpen = true;
            uiPanel.thisPanelController = panelController;
            uiPanel.PanelKey = panelKey;
            uiPanel.OnOpen();
        }

        /// <summary>
        /// 设置关闭，由Controller调用，不要调用
        /// </summary>
        public static void SetClose(UIPanel uiPanel)
        {
            uiPanel.IsOpen = false;
            uiPanel.thisPanelController = null;
            uiPanel.PanelKey = string.Empty;
            uiPanel.OnClose();
        }

        public void Close(bool isDestroy)
        {
            thisPanelController.ClosePanel(PanelKey, isDestroy);
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }
    }
}