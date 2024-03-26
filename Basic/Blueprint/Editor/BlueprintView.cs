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

            RegisterCallback<DragPerformEvent>(DragPerformedCallback);
            RegisterCallback<DragUpdatedEvent>(DragUpdatedCallback);
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

            Blueprint.CheckValid();

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

        public virtual void DeInit()
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
                AddEdge(node);
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

            Blueprint.SetDirtySelf();
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

            string[] pathParam = relativePropertyPath.Split(',');
            string targetPath = pathParam[0];
            SerializedProperty serializedProperty = SerializedObject.FindProperty("nodes").GetArrayElementAtIndex(index)
                .FindPropertyRelative(targetPath);
            for (var i = 1; i < pathParam.Length; i++)
            {
                if (!int.TryParse(pathParam[i], out int targetIndex))
                {
                    CustomLog.LogError($"{nameof(relativePropertyPath)} param err");
                    continue;
                }

                serializedProperty = serializedProperty.GetArrayElementAtIndex(targetIndex);
            }

            SerializedObject.ApplyModifiedProperties();

            return serializedProperty;
        }

        /// <summary>
        /// view中发生部分变化后处理数据（这边是视图已经创建，然后更新数据）
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
                        case BlueprintPortView blueprintPortView:
                            BlueprintNode targetNode =
                                Blueprint.GetNodeByGuid(blueprintPortView.BlueprintPort.OwnerNode.Guid);
                            if (targetNode != null)
                            {
                                targetNode.RemovePort(blueprintPortView.BlueprintPort.PortDirection,
                                    blueprintPortView.BlueprintPort.PortName);
                            }

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
        /// 添加边缘
        /// </summary>
        private void AddEdge(BlueprintEdge blueprintEdge)
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
                node = Blueprint.AddNode(node);
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
        public virtual void RenameParameter(BlueprintBlackboardField field, string newName)
        {
            newName = Blueprint.Blackboard.GetUniqueParameterName(newName);
            field.text = newName;
            field.Parameter.ParameterName = newName;
            List<BlueprintParameterNodeBase> pNode = Blueprint.GetParameterNode(field.Parameter);
            foreach (BlueprintParameterNodeBase parameterNode in pNode)
            {
                if (GetNodeByGuid(parameterNode.Guid) is BlueprintNodeView nodeView)
                {
                    nodeView.RefreshName();
                }
            }

            Blueprint.Blackboard.OnParameterRename();
        }

        /// <summary>
        /// 添加黑板字段
        /// </summary>
        public virtual void AddParameter(BlueprintCreateParameterInfo createInfo)
        {
            IBlueprintParameter parameter = IBlueprintParameter.CreateFromType(createInfo.ParameterType);
            Blueprint.Blackboard.AddParameter(parameter);
            BlackboardView.AddField(parameter);
        }

        /// <summary>
        /// 移除黑板字段
        /// </summary>
        public virtual void RemoveParameter(BlueprintBlackboardField field)
        {
            Blueprint.Blackboard.RemoveParameter(field.Parameter);
            BlackboardView.RemoveField(field);
            List<BlueprintParameterNodeBase> pNode = Blueprint.GetParameterNode(field.Parameter);
            List<Node> deleteNodes = new List<Node>();
            foreach (BlueprintParameterNodeBase parameterNode in pNode)
            {
                Node nodeView = GetNodeByGuid(parameterNode.Guid);
                if (nodeView != null)
                {
                    deleteNodes.Add(nodeView);
                }
            }

            DeleteElements(deleteNodes);
        }

        /// <summary>
        /// 拖拽更新
        /// </summary>
        private void DragUpdatedCallback(DragUpdatedEvent e)
        {
            var dragData = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;
            bool dragging = false;

            if (dragData != null)
            {
                // Handle drag from exposed parameter view
                if (dragData.OfType<BlueprintBlackboardField>().Any())
                {
                    dragging = true;
                }
            }

            if (dragging)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
        }

        /// <summary>
        /// 拖拽参数回调
        /// </summary>
        private void DragPerformedCallback(DragPerformEvent e)
        {
            var mousePos =
                (e.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, e.localMousePosition);

            // Drag and Drop for elements inside the graph
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> dragData)
            {
                var parameterFields = dragData.OfType<BlueprintBlackboardField>();
                foreach (var paramField in parameterFields)
                {
                    // 在鼠标位置生成一个菜单
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Get"), false,
                        () => CreateGetOrSetParameterNode(true, paramField.Parameter));
                    menu.AddItem(new GUIContent("Set"), false,
                        () => CreateGetOrSetParameterNode(false, paramField.Parameter));
                    menu.ShowAsContext();
                }
            }

            void CreateGetOrSetParameterNode(bool isGet, IBlueprintParameter parameter)
            {
                if (isGet)
                {
                    BlueprintGetParameterNode paramNode = BlueprintNode.Create<BlueprintGetParameterNode>();
                    paramNode.OnCreateParameterNode(parameter.Guid);
                    AddNode(paramNode, mousePos, true);
                }
                else
                {
                    if (BlueprintNode.CreateFromType(parameter.SetNodeType) is BlueprintSetParameterNode paramNode)
                    {
                        paramNode.OnCreateParameterNode(parameter.Guid);
                        AddNode(paramNode, mousePos, true);
                    }
                    else
                    {
                        Debug.LogError("Parameter SetNodeType Err");
                    }
                }
            }
        }
    }
}