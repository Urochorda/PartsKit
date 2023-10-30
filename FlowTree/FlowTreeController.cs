using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    /// <summary>
    /// 流程节点状态
    /// </summary>
    public enum FlowTreeNodeState
    {
        Default = 0,
        Running = 1, //运行中
        Fail = 2, //失败
        Success = 3, //成功
    }

    /// <summary>
    /// 流程条件状态
    /// </summary>
    public enum FlowConditionState
    {
        Default = 0, //未设置
        Success = 1, //成功
        Filed = 2, //失败
    }

    /// <summary>
    /// 流程条件数据
    /// </summary>
    [Serializable]
    public class FlowConditionData
    {
        [field: SerializeField] public string Key { get; set; }
        [field: SerializeField] public FlowConditionState Value { get; set; }

        public FlowConditionData(string key, FlowConditionState value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// 流程树数据
    /// </summary>
    [Serializable]
    public class FlowTreeData
    {
        [field: SerializeField] public List<FlowConditionData> ConditionPool { get; set; }
    }

    /// <summary>
    /// 流程控制器
    /// </summary>
    public class FlowTreeController
    {
        public FlowTreeNode RootTreeNode { get; private set; } //根节点
        private FlowTreeNode checkRootTreeNode; //当前检查的根节点
        private FlowTreeData treeData; //流程树数据

        /// <summary>
        /// 初始化流程树
        /// </summary>
        public void Init(FlowTreeData treeDataVal)
        {
            treeData = treeDataVal;
            RootTreeNode = new FlowTreeNode(null, null);
            RootTreeNode.SetState(FlowTreeNodeState.Success, false);
            checkRootTreeNode = RootTreeNode;
            CheckNode(false, true);
        }

        /// <summary>
        /// 设置条件
        /// </summary>
        public void SetCondition(string key, FlowConditionState value)
        {
            int targetConditionIndex = treeData.ConditionPool.FindIndex(item => item.Key == key);
            if (targetConditionIndex < 0)
            {
                treeData.ConditionPool.Add(new FlowConditionData(key, value));
            }

            CheckNode(true, true);
        }

        /// <summary>
        /// 检查流程节点
        /// </summary>
        /// <param name="isTriggerEventInProcess">是否触发检查过程中的事件</param>
        /// <param name="isTriggerEventInEnd">是否触发检查结果的事件</param>
        private void CheckNode(bool isTriggerEventInProcess, bool isTriggerEventInEnd)
        {
            if (!GetNextNode(out FlowTreeNode nextNode))
            {
                //没有下一个节点成功，当前节点的子节点全部进入running状态
                foreach (FlowTreeNode flowNode in checkRootTreeNode.ChildList)
                {
                    if (flowNode.CurState != FlowTreeNodeState.Running)
                    {
                        //最终节点的运行状态肯定会通知事件
                        flowNode.SetState(FlowTreeNodeState.Running, isTriggerEventInEnd);
                    }
                }

                return;
            }

            foreach (FlowTreeNode flowNode in checkRootTreeNode.ChildList)
            {
                if (flowNode == nextNode)
                {
                    continue;
                }

                flowNode.SetState(FlowTreeNodeState.Fail, isTriggerEventInProcess);
            }

            //设置当前成功节点的状态
            nextNode.SetState(FlowTreeNodeState.Success, isTriggerEventInProcess);
            checkRootTreeNode = nextNode;

            //检测下一个节点
            CheckNode(isTriggerEventInProcess, isTriggerEventInEnd);
        }

        /// <summary>
        /// 获取写一个节点（条件达成时可获得下一个节点）
        /// </summary>
        private bool GetNextNode(out FlowTreeNode nextTreeNode)
        {
            nextTreeNode = null;

            //检查成功的节点
            foreach (FlowTreeNode flowNode in checkRootTreeNode.ChildList)
            {
                bool flowNodeIsSuccess = true;
                foreach (FlowConditionData conditionData in flowNode.ConditionKey)
                {
                    int targetConditionIndex = treeData.ConditionPool.FindIndex(item => item.Key == conditionData.Key);
                    if (targetConditionIndex < 0)
                    {
                        flowNodeIsSuccess = false;
                        break;
                    }

                    FlowConditionData targetCondition = treeData.ConditionPool[targetConditionIndex];
                    if (conditionData.Value != targetCondition.Value)
                    {
                        flowNodeIsSuccess = false;
                        break;
                    }
                }

                if (flowNodeIsSuccess)
                {
                    nextTreeNode = flowNode;
                    break;
                }
            }

            return nextTreeNode != null;
        }
    }
}