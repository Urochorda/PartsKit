using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintWindow : EditorWindow
    {
        private BlueprintView blueprintView;

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            if (blueprintView != null)
            {
                blueprintView.SaveBlueprintData();
            }
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
                blueprintView.SaveBlueprintData();
                blueprintView.parent.Remove(blueprintView);
            }

            blueprintView = OnCreateBlueprintView();
            if (blueprintView == null)
            {
                Debug.LogError("GraphView is Null!");
                return;
            }

            OnInitBlueprintView(blueprintView, blueprintVal);
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