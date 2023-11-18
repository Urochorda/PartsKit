using System;
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
            DisposeBlueprintView();
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
                DisposeBlueprintView();
            }

            blueprintView = OnCreateBlueprintView();
            if (blueprintView == null)
            {
                Debug.LogError("GraphView is Null!");
                return;
            }

            OnInitBlueprintView(blueprintView, blueprintVal);
        }

        private void DisposeBlueprintView()
        {
            if (blueprintView == null)
            {
                return;
            }

            blueprintView.Dispose();
            blueprintView.parent.Remove(blueprintView);
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