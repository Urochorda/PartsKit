using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public abstract class LoadPanelFun : MonoBehaviour
    {
        public abstract UIPanel Load(string panelKey);
    }

    public class UIPanelController : MonoBehaviour
    {
        [Serializable]
        private struct UILevelData
        {
            [field: SerializeField] public string LevelKey { get; set; }
            [field: SerializeField] public Transform LevelObj { get; set; }
        }

        [field: SerializeField] public LoadPanelFun CustomLoadPanelFun { get; set; }
        [SerializeField] private List<UILevelData> panelLevels = new List<UILevelData>();
        [SerializeField] private Transform resetPanelParent;

        private readonly Dictionary<string, UIPanel> panelPool = new Dictionary<string, UIPanel>();

        /// <summary>
        /// 打开面板，输出打开的面板对象
        /// </summary>
        /// <param name="panelKey">面板key</param>
        /// <param name="levelKey">面板等级key</param>
        /// <param name="panel">面板对象</param>
        /// <returns></returns>
        public bool OpenPanel<T>(string panelKey, string levelKey, out T panel) where T : UIPanel
        {
            if (!GetPanelLevel(levelKey, out Transform levelObj))
            {
                panel = null;
                return false;
            }

            panelPool.TryGetValue(panelKey, out UIPanel panelVal);
            panel = panelVal as T;
            if (panel == null)
            {
                UIPanel panelPrefab = null;
                if (CustomLoadPanelFun != null)
                {
                    panelPrefab = CustomLoadPanelFun.Load(panelKey);
                }

                if (panelPrefab == null)
                {
                    panelPrefab = Resources.Load<UIPanel>(panelKey);
                }

                if (panelPrefab != null)
                {
                    panel = Instantiate(panelPrefab, levelObj) as T;
                }
            }
            else if (panel.IsOpen)
            {
                ClosePanel(panelKey, false);
            }

            if (panel == null)
            {
                return false;
            }

            panel.transform.SetParent(levelObj);
            panel.transform.SetAsLastSibling();
            panelPool[panelKey] = panel;
            panel.gameObject.SetActive(true);
            panel.SetOpen();
            new ActionRegister(() => DoClosePanel(panelKey, true))
                .UnRegisterWhenGameObjectDestroyed(panel.gameObject);
            return true;
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="panelKey">面板key</param>
        /// <param name="levelKey">面板等级key</param>
        /// <returns></returns>
        public bool OpenPanel(string panelKey, string levelKey)
        {
            return OpenPanel(panelKey, levelKey, out UIPanel _);
        }

        /// <summary>
        /// 打开面板，设置面板数据，输出面板对象
        /// </summary>
        /// <param name="panelKey">面板key</param>
        /// <param name="levelKey">面板等级key</param>
        /// <param name="data">面板数据</param>
        /// <param name="panel">面板对象</param>
        /// <returns></returns>
        public bool OpenPanel<T, TD>(string panelKey, string levelKey, TD data, out T panel) where T : UIPanel<TD>
        {
            if (OpenPanel(panelKey, levelKey, out panel))
            {
                panel.SetData(data);
                return true;
            }

            panel = null;
            return false;
        }

        /// <summary>
        /// 打开面板，设置面板数据
        /// </summary>
        /// <param name="panelKey">面板key</param>
        /// <param name="levelKey">面板等级key</param>
        /// <param name="data">面板数据</param>
        /// <returns></returns>
        public bool OpenPanel<T, TD>(string panelKey, string levelKey, TD data) where T : UIPanel<TD>
        {
            return OpenPanel<T, TD>(panelKey, levelKey, data, out _);
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="panelKey">面板key</param>
        /// <param name="isDestroy">是否销毁</param>
        public void ClosePanel(string panelKey, bool isDestroy)
        {
            if (isDestroy)
            {
                if (panelPool.TryGetValue(panelKey, out UIPanel uiPanel))
                {
                    Destroy(uiPanel.gameObject);
                }
            }
            else
            {
                DoClosePanel(panelKey, false);
            }
        }

        private void DoClosePanel(string panelKey, bool isOriginDestroy)
        {
            if (!panelPool.TryGetValue(panelKey, out UIPanel uiPanel))
            {
                return;
            }

            if (!uiPanel.IsOpen)
            {
                uiPanel.SetClose();
            }

            if (isOriginDestroy)
            {
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
        /// <param name="panelKey">面板key</param>
        /// <param name="panel">面板对象</param>
        /// <returns></returns>
        public bool GetOpenedPanel(string panelKey, out UIPanel panel)
        {
            return panelPool.TryGetValue(panelKey, out panel) && panel != null &&
                   panel.gameObject.activeSelf;
        }

        /// <summary>
        /// 获取panel的等级Transform
        /// </summary>
        /// <param name="levelKey"></param>
        /// <param name="levelObj"></param>
        /// <returns></returns>
        public bool GetPanelLevel(string levelKey, out Transform levelObj)
        {
            levelObj = panelLevels.Find(item => item.LevelKey == levelKey).LevelObj; //结构体不用判空
            return levelObj != null;
        }
    }
}