using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class Blueprint : ScriptableObject
    {
        //可能有派生类，所以用SerializeReference
        [DisplayOnly] [SerializeReference] private List<BlueprintNode> nodes = new List<BlueprintNode>();
        [DisplayOnly] [SerializeReference] private List<BlueprintEdge> edges = new List<BlueprintEdge>();

        public List<BlueprintNode> Nodes => nodes;
        public List<BlueprintEdge> Edges => edges;
        public Stack<BlueprintExecutePort> ExecutePortStack { get; } = new Stack<BlueprintExecutePort>();
        public Stack<BlueprintExecutePort> AllExecutePortStack { get; } = new Stack<BlueprintExecutePort>();

        public event Action OnExecutedUpdate;

        private void OnEnable()
        {
            InitData();
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        private void InitData()
        {
            Nodes.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    Debug.LogError("Add Node Err");
                    return true;
                }

                return false;
            });

            foreach (BlueprintNode node in Nodes)
            {
                node.Init();
            }

            Edges.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    Debug.LogError("Edge Guid Err");
                    return true;
                }

                if (!GetPortByEdge(item, out IBlueprintPort inputPort, out IBlueprintPort outputPort))
                {
                    Debug.LogError("Edge Port Err");
                    return true;
                }

                UpdatePortLink(inputPort, outputPort, true);
                return false;
            });
        }

        public void AddNode(BlueprintNode treeNode)
        {
            if (treeNode == null || Nodes.Contains(treeNode))
            {
                Debug.LogError("Add Node Err");
                return;
            }

            Nodes.Add(treeNode);
            treeNode.Init();
        }

        public void RemoveNode(BlueprintNode treeNode)
        {
            if (treeNode == null)
            {
                return;
            }

            Nodes.Remove(treeNode);
        }

        public BlueprintNode GetNode(string guid)
        {
            return Nodes.Find(item => item != null && item.Guid == guid);
        }

        public void AddEdge(BlueprintEdge edge)
        {
            if (Edges.Contains(edge))
            {
                Debug.LogError("Add Edge Err");
                return;
            }

            Edges.Add(edge);
            if (GetPortByEdge(edge, out IBlueprintPort inputPort, out IBlueprintPort outputPort))
            {
                UpdatePortLink(inputPort, outputPort, true);
            }
        }

        public void RemoveEdge(BlueprintEdge edge)
        {
            Edges.Remove(edge);
            if (GetPortByEdge(edge, out IBlueprintPort inputPort, out IBlueprintPort outputPort))
            {
                UpdatePortLink(inputPort, outputPort, false);
            }
        }

        public List<BlueprintEdge> GetEdgeByPort(IBlueprintPort port)
        {
            if (port == null)
            {
                return new List<BlueprintEdge>();
            }

            return Edges.FindAll(item =>
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
            if (targetPort == null || targetPort.OwnerNode == null || GetNode(targetPort.OwnerNode.Guid) == null)
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

            OnExecutedUpdate?.Invoke();
        }

        private void PushAllExePortStack(BlueprintExecutePort targetPort)
        {
            AllExecutePortStack.Push(targetPort);
            OnExecutedUpdate?.Invoke();
        }

        private bool GetPortByEdge(BlueprintEdge edge, out IBlueprintPort inputPort, out IBlueprintPort outputPort)
        {
            inputPort = null;
            outputPort = null;
            BlueprintNode inputNode = GetNode(edge.InputNodeGuid);
            BlueprintNode outputNode = GetNode(edge.OutputNodeGuid);
            if (inputNode == null || outputNode == null)
            {
                return false;
            }

            inputPort = inputNode.GetPort(IBlueprintPort.Direction.Input, edge.InputPortName);
            outputPort = outputNode.GetPort(IBlueprintPort.Direction.Output, edge.OutputPortName);

            return inputPort != null && outputPort != null;
        }

        private void UpdatePortLink(IBlueprintPort inputPort, IBlueprintPort outputPort, bool add)
        {
            if (add)
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
}