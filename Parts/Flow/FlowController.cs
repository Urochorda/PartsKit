using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
   

    /// <summary>
    /// 流程树数据
    /// </summary>
    [Serializable]
    public class FlowTreeData
    {
        public FlowTree Tree { get; set; }
    }

    /// <summary>
    /// 流程控制器
    /// </summary>
    public class FlowController
    {
        private FlowTreeData treeData; //流程树数据

        /// <summary>
        /// 初始化流程树
        /// </summary>
        public FlowController(FlowTreeData treeDataVal)
        {
            treeData = treeDataVal;
            //CheckNode(false, true);
        }

        // // /// <summary>
        // // /// 设置条件
        // // /// </summary>
        // // public void SetCondition(string key, FlowConditionState value)
        // // {
        // //     int targetConditionIndex = treeData.ConditionPool.FindIndex(item => item.Key == key);
        // //     if (targetConditionIndex < 0)
        // //     {
        // //         treeData.ConditionPool.Add(new FlowConditionData(key, value));
        // //     }
        // //
        // //     CheckNode(true, true);
        // // }
        //
        // /// <summary>
        // /// 检查流程节点
        // /// </summary>
        // /// <param name="isTriggerEventInProcess">是否触发检查过程中的事件</param>
        // /// <param name="isTriggerEventInEnd">是否触发检查结果的事件</param>
        // private void CheckNode(bool isTriggerEventInProcess, bool isTriggerEventInEnd)
        // {
        //     if (!GetNextNode(out FlowNode nextNode))
        //     {
        //         //没有下一个节点成功，当前节点的子节点全部进入running状态
        //         foreach (FlowNode flowNode in checkRootNode.ChildList)
        //         {
        //             if (flowNode.CurState != FlowNodeState.Running)
        //             {
        //                 //最终节点的运行状态肯定会通知事件
        //                 flowNode.SetState(FlowNodeState.Running, isTriggerEventInEnd);
        //             }
        //         }
        //
        //         return;
        //     }
        //
        //     foreach (FlowNode flowNode in checkRootNode.ChildList)
        //     {
        //         if (flowNode == nextNode)
        //         {
        //             continue;
        //         }
        //
        //         flowNode.SetState(FlowNodeState.Fail, isTriggerEventInProcess);
        //     }
        //
        //     //设置当前成功节点的状态
        //     nextNode.SetState(FlowNodeState.Success, isTriggerEventInProcess);
        //     checkRootNode = nextNode;
        //
        //     //检测下一个节点
        //     CheckNode(isTriggerEventInProcess, isTriggerEventInEnd);
        // }
        //
        // /// <summary>
        // /// 获取写一个节点（条件达成时可获得下一个节点）
        // /// </summary>
        // private bool GetNextNode(out FlowNode nextNode)
        // {
        //     nextNode = null;
        //
        //     //检查成功的节点
        //     foreach (FlowNode flowNode in checkRootNode.ChildList)
        //     {
        //         bool flowNodeIsSuccess = true;
        //         foreach (FlowConditionData conditionData in flowNode.ConditionKey)
        //         {
        //             int targetConditionIndex = treeData.ConditionPool.FindIndex(item => item.Key == conditionData.Key);
        //             if (targetConditionIndex < 0)
        //             {
        //                 flowNodeIsSuccess = false;
        //                 break;
        //             }
        //
        //             FlowConditionData targetCondition = treeData.ConditionPool[targetConditionIndex];
        //             if (conditionData.Value != targetCondition.Value)
        //             {
        //                 flowNodeIsSuccess = false;
        //                 break;
        //             }
        //         }
        //
        //         if (flowNodeIsSuccess)
        //         {
        //             nextNode = flowNode;
        //             break;
        //         }
        //     }
        //
        //     return nextNode != null;
        // }
    }
}