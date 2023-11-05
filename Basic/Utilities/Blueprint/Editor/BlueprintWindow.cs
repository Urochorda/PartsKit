using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintWindow : EditorWindow
    {
        public const string GraphWindowStyle = "NodeGroupStyles/NodeGroupWindow";

        private VisualElement rootView;
        private Blueprint blueprint;
        private BlueprintView graphView;

        protected virtual void OnEnable()
        {
            InitRootView();
        }

        protected virtual void OnDisable()
        {
            if (graphView != null)
            {
                graphView.SaveGraphData();
            }
        }

        #region Window

        /// <summary>
        /// 初始化rootView
        /// </summary>
        private void InitRootView()
        {
            rootView = rootVisualElement;
            rootView.name = "graphRootView";
            rootView.styleSheets.Add(Resources.Load<StyleSheet>(GraphWindowStyle));
        }

        /// <summary>
        /// 初始化窗口
        /// </summary>
        public void InitWindow(Blueprint blueprintVal)
        {
            InitGraph(blueprintVal);
        }

        #endregion

        #region Graph

        private void InitGraph(Blueprint blueprintVal)
        {
            if (blueprint != null)
            {
                EditorUtility.SetDirty(blueprint);
                AssetDatabase.SaveAssets();
            }

            if (graphView != null)
            {
                rootView.Remove(graphView);
            }

            blueprint = blueprintVal;
            graphView = OnCreateGraphView(blueprint);
            if (graphView == null)
            {
                Debug.LogError("GraphView is Null!");
                return;
            }

            graphView.Init(blueprint);
            OnInitGraphView(graphView);
        }

        protected virtual BlueprintView OnCreateGraphView(Blueprint blueprintVal)
        {
            return new BlueprintView();
        }

        protected virtual void OnInitGraphView(BlueprintView graphViewVal)
        {
        }

        #endregion
    }
}