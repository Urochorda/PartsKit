using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class Blueprint : ScriptableObject
    {
        public const string CommonNodeGroup = "Blueprint.CommonNodeGroup";

        public const string CommonParameterGroup = "Blueprint.CommonParameterGroup";

        //可能有派生类，所以用SerializeReference
        [DisplayOnly] [SerializeReference] private List<BlueprintNode> nodes = new List<BlueprintNode>();
        [DisplayOnly] [SerializeReference] private List<BlueprintEdge> edges = new List<BlueprintEdge>();
        [DisplayOnly] [SerializeReference] private BlueprintBlackboard blackboard = new BlueprintBlackboard();

        public IReadOnlyList<BlueprintNode> Nodes => nodes;
        public IReadOnlyList<BlueprintEdge> Edges => edges;
        public BlueprintBlackboard Blackboard => blackboard;
        public Stack<BlueprintExecutePort> ExecutePortStack { get; } = new Stack<BlueprintExecutePort>();
        public Stack<BlueprintExecutePort> AllExecutePortStack { get; } = new Stack<BlueprintExecutePort>();
        public GameObject OwnerObject { get; private set; }
        public virtual List<string> NodeGroup { get; } = new List<string>() { CommonNodeGroup };
        public virtual List<string> ParameterGroup { get; } = new List<string>() { CommonParameterGroup };

        public event Action OnExecutedChange;
        bool isEnabled = false;

        protected virtual void OnEnable()
        {
            if (isEnabled)
            {
                OnDisable();
            }

            InitData();
            OnInit();
            isEnabled = true;
        }

        protected virtual void OnDisable()
        {
            if (!isEnabled)
            {
                return;
            }

            OnDeInit();
            isEnabled = false;
        }

        public void CheckValid()
        {
            CheckParametersValid();
            CheckNodeValidPre();
            CheckNodeValid();
            CheckEdgeValidPre();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDeInit()
        {
        }

        private void InitData()
        {
            blackboard.Init(this);
            CheckParametersValid();

            //Node
            CheckNodeValidPre();
            foreach (BlueprintNode node in Nodes)
            {
                node.Init(this);
            }

            CheckNodeValid();

            //Edge
            CheckEdgeValidPre();
            foreach (BlueprintEdge edge in Edges)
            {
                edge.Init();
                UpdateLinkByEdge(edge, true);
            }
        }

        public void SetDirtySelf()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        private void CheckNodeValidPre()
        {
            nodes.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    LogError($"Node Data Err FillName: {name}");
                    SetDirtySelf();
                    return true;
                }

                return false;
            });
        }

        private void CheckNodeValid()
        {
            nodes.RemoveAll(item =>
            {
                if (item.IsNotValid())
                {
                    LogError("Node IsNotValid");
                    SetDirtySelf();
                    return true;
                }

                return false;
            });
        }

        private void CheckEdgeValidPre()
        {
            edges.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    LogError("Edge Guid Err");
                    SetDirtySelf();
                    return true;
                }

                if (!GetPortByEdge(item, out _, out _))
                {
                    LogError("Edge Port Err");
                    SetDirtySelf();
                    return true;
                }

                return false;
            });
        }

        private void CheckParametersValid()
        {
            blackboard.ClearNotValidParameters();
        }

        public virtual BlueprintNode AddNode(BlueprintNode treeNode)
        {
            if (treeNode == null || nodes.Contains(treeNode))
            {
                LogError("Add Node Err");
                return null;
            }

            //先加入列表后init
            nodes.Add(treeNode);
            treeNode.Init(this);
            SetDirtySelf();
            return treeNode;
        }

        public virtual void RemoveNode(BlueprintNode treeNode)
        {
            if (treeNode == null)
            {
                return;
            }

            nodes.Remove(treeNode);
            SetDirtySelf();
        }

        public BlueprintNode GetNodeByGuid(string guid)
        {
            return nodes.Find(item => item != null && item.Guid == guid);
        }

        public List<BlueprintParameterNodeBase> GetParameterNode(IBlueprintParameter parameter)
        {
            List<BlueprintParameterNodeBase> pNodes = new List<BlueprintParameterNodeBase>();
            if (parameter == null)
            {
                return pNodes;
            }

            foreach (BlueprintNode node in nodes)
            {
                if (node is BlueprintParameterNodeBase pItem && pItem.ParameterGuid == parameter.Guid)
                {
                    pNodes.Add(pItem);
                }
            }

            return pNodes;
        }

        public virtual void AddEdge(BlueprintEdge edge)
        {
            if (edges.Contains(edge))
            {
                LogError("Add Edge Err");
                return;
            }

            //先加入列表后init
            edges.Add(edge);
            edge.Init();
            UpdateLinkByEdge(edge, true);
            SetDirtySelf();
        }

        public virtual void RemoveEdge(BlueprintEdge edge)
        {
            edges.Remove(edge);
            UpdateLinkByEdge(edge, false);
            SetDirtySelf();
        }

        public List<BlueprintEdge> GetEdgeByPort(IBlueprintPort port)
        {
            if (port == null)
            {
                return new List<BlueprintEdge>();
            }

            return edges.FindAll(item =>
            {
                switch (port.PortDirection)
                {
                    case IBlueprintPort.Direction.Input:
                        return item.InputPortName == port.PortName;
                    case IBlueprintPort.Direction.Output:
                        return item.OutputPortName == port.PortName;
                }

                return false;
            });
        }

        private bool GetPortByEdge(BlueprintEdge edge, out IBlueprintPort inputPort, out IBlueprintPort outputPort)
        {
            inputPort = null;
            outputPort = null;
            BlueprintNode inputNode = GetNodeByGuid(edge.InputNodeGuid);
            BlueprintNode outputNode = GetNodeByGuid(edge.OutputNodeGuid);
            if (inputNode == null || outputNode == null)
            {
                return false;
            }

            inputPort = inputNode.GetPort(IBlueprintPort.Direction.Input, edge.InputPortName);
            outputPort = outputNode.GetPort(IBlueprintPort.Direction.Output, edge.OutputPortName);

            return inputPort != null && outputPort != null;
        }

        private void UpdateLinkByEdge(BlueprintEdge edge, bool isAdd)
        {
            GetPortByEdge(edge, out IBlueprintPort inputPort, out IBlueprintPort outputPort);
            if (isAdd) //如果时添加的edge则设置edge的port
            {
                edge.InputPort = inputPort;
                edge.OutputPort = outputPort;
            }

            if (inputPort != null && outputPort != null)
            {
                if (isAdd)
                {
                    outputPort.NextPorts.Add(inputPort);
                    inputPort.PrePorts.Add(outputPort);
                }
                else
                {
                    outputPort.NextPorts.Remove(inputPort);
                    inputPort.PrePorts.Remove(outputPort);
                }
            }
        }

        #region Executed

        public bool IsExecuting()
        {
            return ExecutePortStack.Count > 0;
        }

        public void BeginExecuted(BlueprintExecutePort targetPort)
        {
            DoExecuted(targetPort);
        }

        public void UpdateExecuted()
        {
            ResetAllExePortStack();
            if (ExecutePortStack.Count <= 0)
            {
                return;
            }

            BlueprintExecutePort startPort = ExecutePortStack.Pop();
            DoExecuted(startPort);
        }

        public void EndExecuted()
        {
            ExecutePortStack.Clear();
            ResetAllExePortStack();
        }

        private void DoExecuted(BlueprintExecutePort targetPort)
        {
            if (targetPort == null || targetPort.OwnerNode == null || GetNodeByGuid(targetPort.OwnerNode.Guid) == null)
            {
                return;
            }

            ExecutePortStack.Push(targetPort);
            PushAllExePortStack(targetPort);
            targetPort.Execute(out BlueprintExecutePort nextPort);
            if (nextPort != null) //有下一个则执行下一个
            {
                DoExecuted(nextPort);
                return;
            }

            //到底了回溯堆栈
            while (ExecutePortStack.Count > 0)
            {
                BlueprintExecutePort portItem = ExecutePortStack.Peek();
                switch (portItem.ExecuteState)
                {
                    case BlueprintExecuteState.Running:
                        //移除后重新执行
                        ExecutePortStack.Pop();
                        DoExecuted(portItem);
                        return;
                    case BlueprintExecuteState.Wait:
                        //结束执行
                        return;
                    default:
                    case BlueprintExecuteState.End:
                        //节点结束，移除跳过
                        ExecutePortStack.Pop();
                        break;
                }
            }
        }

        private void ResetAllExePortStack()
        {
            AllExecutePortStack.Clear();
            foreach (BlueprintExecutePort curExecutePort in ExecutePortStack)
            {
                AllExecutePortStack.Push(curExecutePort);
            }

            OnExecutedChange?.Invoke();
        }

        private void PushAllExePortStack(BlueprintExecutePort targetPort)
        {
            AllExecutePortStack.Push(targetPort);
            OnExecutedChange?.Invoke();
        }

        public void SetOwnerObject(GameObject ownerObjectVal)
        {
            OwnerObject = ownerObjectVal;
        }

        #endregion

        /// <summary>
        /// 克隆自己
        /// </summary>
        private Blueprint CloneSelf()
        {
            string selfJson = JsonUtility.ToJson(this);
            Blueprint cloneObj = CreateInstance(GetType()) as Blueprint;
            if (cloneObj == null)
            {
                return null;
            }

            cloneObj.name = "Temp";
            JsonUtility.FromJsonOverwrite(selfJson, cloneObj);
            cloneObj.OnDisable();
            cloneObj.OnEnable();
            return cloneObj;
        }

        public T GetRunBlueprint<T>(string runName) where T : Blueprint
        {
            T runB = CloneSelf() as T;

            if (runB != null)
            {
                runB.name = runName;
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
            if (runB != null)
            {
                runBlueprints.Add(runB);
            }
#endif
            return runB;
        }

        public void LogError(string info)
        {
            CustomLog.LogError($"FillName: {name}, {info}");
        }

#if UNITY_EDITOR

        public event Action OnEditorReset;
        private readonly List<Blueprint> runBlueprints = new List<Blueprint>();
        public IReadOnlyList<Blueprint> RunBlueprints => runBlueprints;

        private void OnEditorPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                EditorResetData();
            }
        }

        private void EditorResetData()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
            foreach (Blueprint runBlueprint in runBlueprints)
            {
                runBlueprint.OnEditorReset?.Invoke();
                runBlueprint.OnEditorReset = null;
            }

            runBlueprints.Clear();
        }

#endif
    }
}