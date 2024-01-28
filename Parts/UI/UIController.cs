using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public abstract class LoadUIPanelFun : MonoBehaviour
    {
        public abstract T Load<T>(string panelKey) where T : UIPanel;
        public abstract void LoadAsync<T>(string panelKey, Action<T> onPanelLoad) where T : UIPanel;
    }

    public class UIController : PartsKitBehaviour
    {
        [Serializable]
        private struct UILevelData
        {
            [field: SerializeField] public string LevelKey { get; set; }
            [field: SerializeField] public Transform LevelObj { get; set; }
        }

        [field: SerializeField] public LoadUIPanelFun CustomLoadPanelFun { get; set; }

        [SerializeField] [Tooltip("CustomLoadPanelFun为空时默认使用Resources的加载方式")]
        private string resourcesPath = "UIPanel";

        [SerializeField] private List<UILevelData> panelLevels = new List<UILevelData>();
        [SerializeField] private Transform resetPanelParent;

        private readonly Dictionary<string, UIPanel> panelPool = new Dictionary<string, UIPanel>();

        protected override void OnInit()
        {
        }

        protected override void OnDeInit()
        {
        }

        /// <summary>
        /// 预加载Panel
        /// </summary>
        public void PreLoadPanel<T>(string panelKey, Action<T> onPanelLoad) where T : UIPanel
        {
            LoadUIPanelAsync(panelKey, onPanelLoad);
        }

        public void PreLoadPanel(string panelKey)
        {
            LoadUIPanelAsync<UIPanel>(panelKey, null);
        }

        /// <summary>
        /// 打开面板，输出打开的面板对象
        /// </summary>
        public bool OpenPanel<T>(string panelKey, string levelKey, out T panel) where T : UIPanel
        {
            if (!GetPanelLevel(levelKey, out Transform levelObj))
            {
                CustomLog.LogError($"{nameof(OpenPanelAsync)} {levelKey} err");
                panel = null;
                return false;
            }

            panelPool.TryGetValue(panelKey, out UIPanel panelVal);
            panel = panelVal as T;
            if (panel == null)
            {
                if (!CreateUIPanel(panelKey, levelObj, out panel))
                {
                    return false;
                }
            }

            SetUIPanelOpen(panelKey, panel, levelObj);

            return true;
        }

        public bool OpenPanel(string panelKey, string levelKey)
        {
            return OpenPanel<UIPanel>(panelKey, levelKey, out _);
        }

        /// <summary>
        /// 异步打开panel
        /// </summary>
        public void OpenPanelAsync<T>(string panelKey, string levelKey, Action<T> onPanelOpen) where T : UIPanel
        {
            if (!GetPanelLevel(levelKey, out Transform levelObj))
            {
                CustomLog.LogError($"{nameof(OpenPanelAsync)} {levelKey} err");
                return;
            }

            panelPool.TryGetValue(panelKey, out UIPanel panelVal);
            if (panelVal is T openPanel)
            {
                SetUIPanelOpen(panelKey, openPanel, levelObj);
                onPanelOpen?.Invoke(openPanel);
            }
            else
            {
                CreateUIPanelAsync<T>(panelKey, levelObj, (createPanel) =>
                {
                    SetUIPanelOpen(panelKey, createPanel, levelObj);
                    onPanelOpen?.Invoke(createPanel);
                });
            }
        }

        public void OpenPanelAsync(string panelKey, string levelKey)
        {
            OpenPanelAsync<UIPanel>(panelKey, levelKey, null);
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="panelKey">面板key</param>
        /// <param name="isDestroy">是否销毁</param>
        public void ClosePanel(string panelKey, bool isDestroy)
        {
            if (!panelPool.TryGetValue(panelKey, out UIPanel uiPanel))
            {
                return;
            }

            if (uiPanel.IsOpen)
            {
                UIPanel.SetClose(uiPanel);
            }

            if (isDestroy)
            {
                Destroy(uiPanel.gameObject);
                panelPool.Remove(panelKey);
            }
            else
            {
                uiPanel.gameObject.SetActive(false);
                uiPanel.transform.SetParent(resetPanelParent);
            }
        }

        /// <summary>
        /// 获取打开的面板
        /// </summary>
        public bool GetOpenedPanel<T>(string panelKey, out T panel) where T : UIPanel
        {
            if (!panelPool.TryGetValue(panelKey, out UIPanel panelVal) || panelVal is not T tPanel || !tPanel.IsOpen)
            {
                panel = null;
                return false;
            }

            panel = tPanel;
            return true;
        }

        /// <summary>
        /// 获取panel的等级Transform
        /// </summary>
        public bool GetPanelLevel(string levelKey, out Transform levelObj)
        {
            levelObj = panelLevels.Find(item => item.LevelKey == levelKey).LevelObj; //结构体不用判空
            return levelObj != null;
        }

        private bool CreateUIPanel<T>(string panelKey, Transform levelObj, out T uiPanel) where T : UIPanel
        {
            T panelPrefab = CustomLoadPanelFun != null
                ? CustomLoadPanelFun.Load<T>(panelKey)
                : Resources.Load<T>(GetResourcesPath(panelKey));

            uiPanel = panelPrefab != null ? InstantiateUIPanel(panelPrefab, levelObj) : null;
            return uiPanel != null;
        }

        private void CreateUIPanelAsync<T>(string panelKey, Transform levelObj, Action<T> onPanelLoad) where T : UIPanel
        {
            LoadUIPanelAsync<T>(panelKey, (panelPrefab) =>
            {
                T uiPanel = InstantiateUIPanel(panelPrefab, levelObj);
                onPanelLoad?.Invoke(uiPanel);
            });
        }

        private string GetResourcesPath(string panelKey)
        {
            return $"{resourcesPath}/{panelKey}";
        }

        private void LoadUIPanelAsync<T>(string panelKey, Action<T> onPanelLoad) where T : UIPanel
        {
            if (CustomLoadPanelFun != null)
            {
                CustomLoadPanelFun.LoadAsync<T>(panelKey, (panelPrefab) =>
                {
                    if (panelPrefab == null)
                    {
                        CustomLog.LogError($"{nameof(LoadUIPanelAsync)} {panelKey} err");
                        return;
                    }

                    onPanelLoad?.Invoke(panelPrefab);
                });
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync<T>(GetResourcesPath(panelKey));
                request.completed += (operation) => { onPanelLoad?.Invoke(request.asset as T); };
            }
        }

        private T InstantiateUIPanel<T>(T prefab, Transform levelObj) where T : UIPanel
        {
            T uiPanel = Instantiate(prefab, levelObj);

            if (uiPanel != null)
            {
                UIPanel.SetCreate(uiPanel);
            }

            return uiPanel;
        }

        private void SetUIPanelOpen(string panelKey, UIPanel panel, Transform levelObj)
        {
            if (panel.IsOpen)
            {
                ClosePanel(panelKey, false);
            }

            panel.transform.SetParent(levelObj);
            panel.transform.SetAsLastSibling();
            panelPool[panelKey] = panel;
            panel.gameObject.SetActive(true);
            UIPanel.SetOpen(panel, this, panelKey);
        }
    }
}