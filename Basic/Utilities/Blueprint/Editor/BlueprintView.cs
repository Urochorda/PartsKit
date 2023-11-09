using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintView : GraphView
    {
        private const string DefaultStylePath = "Styles/BlueprintView";
        private Blueprint blueprint;
        private BlueprintWindow window;
        private readonly List<Port> compatiblePorts = new List<Port>();

        public BlueprintView()
        {
            BasicSetting();
        }

        /// <summary>
        /// 基础设置，与数据无关
        /// </summary>
        private void BasicSetting()
        {
            //接在style文件
            StyleSheet styleSheet = Resources.Load<StyleSheet>(DefaultStylePath);
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            // 添加背景网格
            Insert(0, new GridBackground());

            //注册功能
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(0.05f, 2f); //简单控制缩放即可
        }

        /// <summary>
        /// 根据数据初始化
        /// </summary>
        public void Init(BlueprintWindow windowVal, Blueprint blueprintVal)
        {
            blueprint = blueprintVal;
            window = windowVal;
            if (blueprint == null)
            {
                return;
            }

            //等所有view都初始化完毕后再设置，避免初始化引起的变化导致数据错乱
            graphViewChanged = null;
            //初始化view
            InitView();
            //注册创建节点事件，注册后会自动在BuildContextualMenu中绘制CreateNode按钮
            nodeCreationRequest = (c) => { ShowNodeSearchWindow(c.screenMousePosition); };
            graphViewChanged = GraphViewChangedCallback;
        }

        private void InitView()
        {
            foreach (BlueprintNode node in blueprint.Nodes)
            {
                AddNode(node, Vector2.zero, false);
            }

            foreach (BlueprintEdge node in blueprint.Edges)
            {
                CreateEdge(node);
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveBlueprintData()
        {
            if (blueprint == null)
            {
                return;
            }

            EditorUtility.SetDirty(blueprint);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 绘制菜单按钮
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
        }

        /// <summary>
        /// view中任意变化后处理数据
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        private GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
        {
            if (changes.elementsToRemove != null)
            {
                foreach (GraphElement element in changes.elementsToRemove)
                {
                    switch (element)
                    {
                        case BlueprintNodeView blueprintNodeView:
                            blueprint.RemoveNode(blueprintNodeView.BlueprintNode);
                            break;
                        case BlueprintEdgeView blueprintEdgeView:
                            blueprint.RemoveEdge(blueprintEdgeView.BlueprintEdge);
                            break;
                    }
                }
            }

            if (changes.movedElements != null)
            {
                foreach (GraphElement element in changes.movedElements)
                {
                    switch (element)
                    {
                        case BlueprintNodeView blueprintNodeView:
                            blueprintNodeView.BlueprintNode.Rect = blueprintNodeView.GetPosition();
                            break;
                    }
                }
            }

            if (changes.edgesToCreate != null)
            {
                foreach (Edge edgeView in changes.edgesToCreate)
                {
                    if (edgeView is not BlueprintEdgeView blueprintEdgeView)
                    {
                        continue;
                    }

                    BlueprintPortView inputPortView = blueprintEdgeView.input as BlueprintPortView;
                    BlueprintPortView outputPortView = blueprintEdgeView.output as BlueprintPortView;
                    if (inputPortView == null || outputPortView == null)
                    {
                        continue;
                    }

                    BlueprintEdge blueprintEdge =
                        BlueprintEdge.CreateBlueprintEdge(inputPortView.BlueprintPort, outputPortView.BlueprintPort);
                    blueprintEdgeView.Init(blueprintEdge);
                    blueprint.AddEdge(blueprintEdge);
                }
            }

            return changes;
        }

        /// <summary>
        /// 获取可连线列表
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            compatiblePorts.Clear();
            foreach (Port port in ports)
            {
                //自身节点的port
                if (port.node == startPort.node)
                {
                    continue;
                }

                //相同输入，输出的port
                if (port.direction == startPort.direction)
                {
                    continue;
                }

                //数据类型不匹配
                if (port.portType != startPort.portType)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        /// <summary>
        /// 获取节点创建搜索列表
        /// </summary>
        /// <returns></returns>
        protected virtual List<SearcherItem> GetNodeSearcherItems()
        {
            return new List<SearcherItem>();
        }

        /// <summary>
        /// 展示搜索框
        /// </summary>
        private void ShowNodeSearchWindow(Vector2 screenMousePosition)
        {
            ScreenMousePositionTo(screenMousePosition, out Vector2 windowMousePosition,
                out Vector2 graphMousePosition);

            SearcherWindow.Show(window, GetNodeSearcherItems(), "Create Node",
                item => CreateNode(item, graphMousePosition), windowMousePosition);
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        private bool CreateNode(SearcherItem searcherItem, Vector2 graphMousePosition)
        {
            if (searcherItem == null)
            {
                return false;
            }

            BlueprintNode blueprintNode = BlueprintNode.CreateFromType((Type)searcherItem.UserData);
            blueprintNode.OnCreateByEditorView();
            AddNode(blueprintNode, graphMousePosition, true);
            return true;
        }

        /// <summary>
        /// 创建边缘（这里是手动创建）
        /// </summary>
        private void CreateEdge(BlueprintEdge blueprintEdge)
        {
            BlueprintNodeView inputNodeView = GetNodeByGuid(blueprintEdge.InputNodeGuid) as BlueprintNodeView;
            BlueprintNodeView outputNodeView = GetNodeByGuid(blueprintEdge.OutputNodeGuid) as BlueprintNodeView;
            if (inputNodeView == null || outputNodeView == null)
            {
                Debug.LogError("Create Edge Node Err");
                return;
            }

            BlueprintPortView inputPort = inputNodeView.GetPortView(Direction.Input, blueprintEdge.InputPortName);
            BlueprintPortView outputPort = outputNodeView.GetPortView(Direction.Output, blueprintEdge.OutputPortName);

            if (inputPort == null || outputPort == null)
            {
                Debug.LogError("Create Edge Port Err");
                return;
            }

            BlueprintEdgeView edge = inputPort.ConnectTo<BlueprintEdgeView>(outputPort);
            edge.Init(blueprintEdge);
            AddElement(edge);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        private void AddNode(BlueprintNode node, Vector2 graphMousePosition, bool isAddData)
        {
            Type viewType = BlueprintTypeCache.GetNodeViewType(node.GetType());
            object viewObj = Activator.CreateInstance(viewType);
            if (viewObj is not BlueprintNodeView nodeView)
            {
                return;
            }

            if (isAddData)
            {
                Rect nodePos = new Rect(graphMousePosition, new Vector2(100, 100));
                //设置数据存储
                node.Rect = nodePos;
                blueprint.AddNode(node);
            }

            //创建view
            nodeView.Init(node);
            AddElement(nodeView);
        }

        /// <summary>
        /// 屏幕坐标转换
        /// </summary>
        private void ScreenMousePositionTo(Vector2 screenMousePosition, out Vector2 windowMousePosition,
            out Vector2 graphMousePosition)
        {
            var windowRoot = window.rootVisualElement;
            windowMousePosition =
                windowRoot.ChangeCoordinatesTo(windowRoot.parent, screenMousePosition - window.position.position);
            graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
        }
    }
}