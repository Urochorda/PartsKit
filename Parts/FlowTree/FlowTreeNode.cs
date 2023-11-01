using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class FlowTreeNode
    {
        public event Action<FlowTreeNode> OnStateChange;
        public FlowTreeNodeState CurState { get; private set; } = FlowTreeNodeState.Default;
        public List<FlowConditionData> ConditionKey { get; private set; } = new List<FlowConditionData>();
        public FlowTreeNode ParentTreeNode { get; private set; }
        public List<FlowTreeNode> ChildList { get; } = new List<FlowTreeNode>();

        public FlowTreeNode(List<FlowConditionData> conditionKeyVal, Action<FlowTreeNode> onStateChangeVal)
        {
            OnStateChange = null;
            if (conditionKeyVal != null)
            {
                ConditionKey = conditionKeyVal;
            }

            if (conditionKeyVal != null)
            {
                OnStateChange += onStateChangeVal;
            }
        }

        public void SetState(FlowTreeNodeState state, bool isTriggerEvent)
        {
            CurState = state;
            if (isTriggerEvent)
            {
                OnStateChange?.Invoke(this);
            }
        }

        public void AddChild(FlowTreeNode childTreeNode)
        {
            if (childTreeNode == null || childTreeNode.ParentTreeNode == this) //已经是子节点了
            {
                return;
            }

            if (childTreeNode.ParentTreeNode != null) //添加的子节点有旧的父节点
            {
                childTreeNode.ParentTreeNode.RemoveChild(childTreeNode);
            }

            ChildList.Add(childTreeNode);
            childTreeNode.ParentTreeNode = this;
        }

        public void RemoveChild(FlowTreeNode childTreeNode)
        {
            if (childTreeNode == null || childTreeNode.ParentTreeNode != this) //不是自己的子节点
            {
                return;
            }

            ChildList.Remove(childTreeNode);
            childTreeNode.ParentTreeNode = null;
        }

        public void SetParent(FlowTreeNode parentTreeNode)
        {
            if (parentTreeNode == null)
            {
                if (ParentTreeNode != null)
                {
                    ParentTreeNode.RemoveChild(this);
                }
            }
            else
            {
                parentTreeNode.AddChild(this); //将自己添加到目标节点的子节点中
            }
        }
    }
}