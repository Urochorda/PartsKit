using System;
using System.Collections.Generic;
using System.Linq;
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
        public BlueprintWindow Window { get; private set; }
        public Blueprint Blueprint { get; private set; }
        public SerializedObject SerializedObject { get; private set; }
        public BlueprintBlackboardView BlackboardView { get; private set; }

        public Func<IEnumerable<SearcherItem>> OnGetNodeSearcherItems { get; set; }
        private readonly List<Port> compatiblePorts = new List<Port>();
        private readonly List<BlueprintExecutePort> lastExecutePorts = new List<BlueprintExecutePort>();

        protected override bool canCopySelection => selection.Any(e => e is BlueprintNodeView);
        protected override bool canCutSelection => selection.Any(e => e is BlueprintNodeView);

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
        public virtual void Init(BlueprintWindow windowVal, Blueprint blueprintVal)
        {
            Blueprint = blueprintVal;
            Window = windowVal;
            if (Blueprint == null)
            {
                return;
            }

            SerializedObject = new SerializedObject(blueprintVal);

            //等所有view都初始化完毕后再设置，避免初始化引起的变化导致数据错乱
            graphViewChanged = null;
            //初始化view
            InitView();
            //注册创建节点事件，注册后会自动在BuildContextualMenu中绘制CreateNode按钮
            nodeCreationRequest = (c) => { ShowNodeSearchWindow(c.screenMousePosition); };
            graphViewChanged = GraphViewChangedCallback;
            Blueprint.OnExecutedChange += DrawExecuteLine;
        }

        public virtual void Dispose()
        {
            SaveBlueprintData();
            if (Blueprint != null)
            {
                Blueprint.OnExecutedChange -= DrawExecuteLine;
            }
        }

        private void InitView()
        {
            foreach (BlueprintNode node in Blueprint.Nodes)
            {
                AddNode(node, Vector2.zero, false);
            }

            foreach (BlueprintEdge node in Blueprint.Edges)
            {
                CreateEdge(node);
            }

            CreateBlackboardView();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public virtual void SaveBlueprintData()
        {
            if (Blueprint == null)
            {
                return;
            }

            EditorUtility.SetDirty(Blueprint);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 和获取节点的属性
        /// </summary>
        public SerializedProperty FindNodeProperty(BlueprintNode node, string relativePropertyPath)
        {
            SerializedObject.Update();
            int index = -1;
            int macCount = Blueprint.Nodes.Count;
            for (int i = 0; i < macCount; i++)
            {
                BlueprintNode item = Blueprint.Nodes[i];
                if (item.Guid == node.Guid)
                {
                    index = i;
                    break;
                }
            }

            SerializedProperty serializedProperty = SerializedObject.FindProperty("nodes").GetArrayElementAtIndex(index)
                .FindPropertyRelative(relativePropertyPath);
            SerializedObject.ApplyModifiedProperties();

            return serializedProperty;
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
                            Blueprint.RemoveNode(blueprintNodeView.BlueprintNode);
                            break;
                        case BlueprintEdgeView blueprintEdgeView:
                            Blueprint.RemoveEdge(blueprintEdgeView.BlueprintEdge);
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

                    //先将数据设置好，再去初始化视图
                    BlueprintEdge blueprintEdge =
                        BlueprintEdge.CreateBlueprintEdge(inputPortView.BlueprintPort, outputPortView.BlueprintPort);
                    Blueprint.AddEdge(blueprintEdge);

                    blueprintEdgeView.Init(blueprintEdge);
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
        private IEnumerable<SearcherItem> GetNodeSearcherItems()
        {
            IEnumerable<SearcherItem> targetList = OnGetNodeSearcherItems?.Invoke();
            return targetList ?? new List<SearcherItem>();
        }

        /// <summary>
        /// 展示搜索框
        /// </summary>
        private void ShowNodeSearchWindow(Vector2 screenMousePosition)
        {
            ScreenMousePositionTo(screenMousePosition, out Vector2 windowMousePosition,
                out Vector2 graphMousePosition);

            SearcherWindow.Show(Window, GetNodeSearcherItems().ToList(), "Create Node",
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

            //先将数据设置好，再去初始化视图
            if (isAddData)
            {
                Rect nodePos = new Rect(graphMousePosition, new Vector2(100, 100));
                //设置数据存储
                node.Rect = nodePos;
                Blueprint.AddNode(node);
            }

            //创建view
            nodeView.Init(node, this);
            AddElement(nodeView);
        }

        /// <summary>
        /// 屏幕坐标转换
        /// </summary>
        private void ScreenMousePositionTo(Vector2 screenMousePosition, out Vector2 windowMousePosition,
            out Vector2 graphMousePosition)
        {
            var windowRoot = Window.rootVisualElement;
            windowMousePosition =
                windowRoot.ChangeCoordinatesTo(windowRoot.parent, screenMousePosition - Window.position.position);
            graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
        }

        /// <summary>
        /// 绘制执行线
        /// </summary>
        private void DrawExecuteLine()
        {
            if (Blueprint == null)
            {
                return;
            }

            for (var i = lastExecutePorts.Count - 1; i >= 0; i--)
            {
                BlueprintExecutePort lastExePort = lastExecutePorts[i];
                if (Blueprint.AllExecutePortStack.Contains(lastExePort))
                {
                    continue;
                }

                SetBPortExecuteState(lastExePort, false);
                lastExecutePorts.RemoveAt(i);
            }

            foreach (BlueprintExecutePort lastExePort in Blueprint.AllExecutePortStack)
            {
                if (lastExecutePorts.Contains(lastExePort))
                {
                    continue;
                }

                SetBPortExecuteState(lastExePort, true);
                lastExecutePorts.Add(lastExePort);
            }

            void SetBPortExecuteState(BlueprintExecutePort exePortData, bool state)
            {
                BlueprintPortView bPortView = GetBPortView(exePortData);
                if (bPortView != null)
                {
                    bPortView.SetExecuteState(state);
                    List<BlueprintEdge> edgeList = Blueprint.GetEdgeByPort(exePortData);
                    foreach (BlueprintEdge edge in edgeList)
                    {
                        Edge edgeView = GetEdgeByGuid(edge.Guid);
                        if (edgeView != null)
                        {
                            edgeView.UpdateEdgeControl();
                        }
                    }
                }
            }

            BlueprintPortView GetBPortView(BlueprintExecutePort exePortData)
            {
                Node nodeView = GetNodeByGuid(exePortData.OwnerNode.Guid);
                if (nodeView is not BlueprintNodeView bNodeView)
                {
                    return null;
                }

                BlueprintPortView bPortView = bNodeView.GetPortView(exePortData.PortDirection.ToEditorDirection(),
                    exePortData.PortName);
                return bPortView;
            }
        }

        /// <summary>
        /// 创建黑板view
        /// </summary>
        private void CreateBlackboardView()
        {
            BlackboardView = new BlueprintBlackboardView();
            Add(BlackboardView);
            BlackboardView.Init(this, Blueprint.Blackboard);
        }

        /// <summary>
        /// 黑板字段重命名
        /// </summary>
        public virtual void RenameBlackboardParameter(BlueprintBlackboardField field, string newName)
        {
            newName = Blueprint.Blackboard.GetUniqueParameterName(newName);
            field.text = newName;
            field.Parameter.ParameterName = newName;
        }

        /// <summary>
        /// 添加黑板字段
        /// </summary>
        public virtual void AddBlackboardParameter(BlueprintCreateParameterInfo createInfo)
        {
            IBlueprintParameter parameter = IBlueprintParameter.CreateFromType(createInfo.ParameterType);
            Blueprint.Blackboard.AddParameter(parameter);
            BlackboardView.AddBlackboardField(parameter);
        }

        /// <summary>
        /// 移除黑板字段
        /// </summary>
        public virtual void RemoveBlackboardParameter(BlueprintBlackboardField field)
        {
            Blueprint.Blackboard.RemoveParameter(field.Parameter);
            BlackboardView.RemoveBlackboardField(field);
        }
    }
}