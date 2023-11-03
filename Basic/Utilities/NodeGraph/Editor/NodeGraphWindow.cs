using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class NodeGraphWindow : EditorWindow
    {
        public const string GraphWindowStyle = "NodeGroupStyles/NodeGroupWindow";

        private VisualElement rootView;
        private GraphData graphData;
        private NodeGraphView graphView;

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
        public void InitWindow(GraphData graphDataVal)
        {
            InitGraph(graphDataVal);
        }

        #endregion

        #region Graph

        private void InitGraph(GraphData graphDataVal)
        {
            if (graphData != null)
            {
                EditorUtility.SetDirty(graphData);
                AssetDatabase.SaveAssets();
            }

            if (graphView != null)
            {
                rootView.Remove(graphView);
            }

            graphData = graphDataVal;
            graphView = OnCreateGraphView(graphData);
            if (graphView == null)
            {
                Debug.LogError("GraphView is Null!");
                return;
            }

            graphView.Init(graphData);
            OnInitGraphView(graphView);
        }

        protected virtual NodeGraphView OnCreateGraphView(GraphData graphDataVal)
        {
            return new NodeGraphView();
        }

        protected virtual void OnInitGraphView(NodeGraphView graphViewVal)
        {
        }

        #endregion
    }
}