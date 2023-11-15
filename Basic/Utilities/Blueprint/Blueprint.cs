using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class Blueprint : ScriptableObject
    {
        //可能有派生类，所以用SerializeReference
        [field: DisplayOnly]
        [field: SerializeReference]
        public List<BlueprintNode> Nodes { get; private set; } = new List<BlueprintNode>();

        [field: DisplayOnly]
        [field: SerializeReference]
        public List<BlueprintEdge> Edges { get; private set; } = new List<BlueprintEdge>();

        public Stack<BlueprintExecutePort> ExecutePortStack { get; } = new Stack<BlueprintExecutePort>();
        public Stack<BlueprintExecutePort> AllExecutePortStack { get; } = new Stack<BlueprintExecutePort>();

        public event Action OnExecutedUpdate;

        private void OnEnable()
        {
            InitNode();
            InitPort();
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        private void InitNode()
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
        }

        private void InitPort()
        {
            Edges.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    Debug.LogError("Edge Guid Err");
                    return true;
                }

                BlueprintNode inputNode = GetNode(item.InputNodeGuid);
                BlueprintNode outputNode = GetNode(item.OutputNodeGuid);
                if (inputNode == null || outputNode == null)
                {
                    Debug.LogError("Edge Node Err");
                    return true;
                }

                IBlueprintPort inputPort = inputNode.GetPort(IBlueprintPort.Direction.Input, item.InputPortName);
                IBlueprintPort outputPort = outputNode.GetPort(IBlueprintPort.Direction.Output, item.OutputPortName);
                if (inputPort == null || outputPort == null)
                {
                    Debug.LogError("Edge Port Err");
                    return true;
                }

                outputPort.NextPorts.Add(inputPort);
                inputPort.PrePorts.Add(inputPort);

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
        }

        public void RemoveEdge(BlueprintEdge edge)
        {
            Edges.Remove(edge);
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
    }
}