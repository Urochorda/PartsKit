using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintWindow : EditorWindow
    {
        private BlueprintView blueprintView;

        protected virtual void OnDestroy()
        {
            DeInitBlueprintView();
        }

        #region Window

        /// <summary>
        /// 根据数据初始化窗口
        /// </summary>
        public void InitWindow(Blueprint blueprintVal)
        {
            InitBlueprintView(blueprintVal);
        }

        #endregion

        #region BlueprintView

        /// <summary>
        /// 根据数据初始化
        /// </summary>
        private void InitBlueprintView(Blueprint blueprintVal)
        {
            if (blueprintView != null)
            {
                DeInitBlueprintView();
            }

            blueprintView = OnCreateBlueprintView();
            if (blueprintView == null)
            {
                Debug.LogError("GraphView is Null!");
                return;
            }
#if UNITY_EDITOR
            blueprintVal.OnEditorReset += DeInitBlueprintView;
#endif
            OnInitBlueprintView(blueprintView, blueprintVal);
        }

        private void DeInitBlueprintView()
        {
            if (blueprintView == null)
            {
                return;
            }

#if UNITY_EDITOR
            blueprintView.Blueprint.OnEditorReset -= DeInitBlueprintView;
#endif
            blueprintView.DeInit();
            blueprintView.parent.Remove(blueprintView);
            blueprintView = null;
        }

        /// <summary>
        /// 创建BlueprintView，默认创建BlueprintView，如果扩展BlueprintView，可重写生成方法
        /// </summary>
        protected virtual BlueprintView OnCreateBlueprintView()
        {
            BlueprintView bv = new BlueprintView();
            rootVisualElement.Add(bv);
            bv.StretchToParentSize();
            return bv;
        }

        /// <summary>
        /// 初始化blueprintView
        /// </summary>
        protected virtual void OnInitBlueprintView(BlueprintView blueprintViewVal, Blueprint blueprintVal)
        {
            blueprintViewVal.Init(this, blueprintVal);
        }

        #endregion
    }
}