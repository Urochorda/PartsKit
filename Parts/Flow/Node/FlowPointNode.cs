using System;
using UnityEngine;

namespace PartsKit
{
    /// <summary>
    /// 流程节点状态
    /// </summary>
    public enum FlowPointState
    {
        Running = 1, //运行中
        Fail = 2, //失败
        Success = 3, //成功
    }

    [Serializable]
    [FlowCreateNode("Common", CreateName, "流程节点")]
    public class FlowPointNode : BlueprintNode
    {
        private const string CreateName = "Point (wait for running)";
        public override string NodeName => CreateName;
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort RunningExePort { get; private set; }
        public BlueprintExecutePort FailExePort { get; private set; }
        public BlueprintExecutePort SuccessExePort { get; private set; }
        public BlueprintValuePort<FlowPointState> ConditionPort { get; private set; }

        private BlueprintExecutePortResult executePortResult;

        [SerializeField] public FlowPointState condition = FlowPointState.Success;

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, OnInputExecuted);
            RunningExePort = BlueprintPortUtility.CreateExecutePort("RunningExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);
            FailExePort = BlueprintPortUtility.CreateExecutePort("FailExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);
            SuccessExePort = BlueprintPortUtility.CreateExecutePort("SuccessExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);
            ConditionPort = BlueprintPortUtility.CreateValuePort<FlowPointState>("Condition",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, nameof(condition),
                GetConditionValue);

            AddPort(InputExePort);
            AddPort(RunningExePort);
            AddPort(FailExePort);
            AddPort(SuccessExePort);
            AddPort(ConditionPort);
        }

        private BlueprintExecutePortResult OnInputExecuted(BlueprintExecutePort executePort)
        {
            ConditionPort.GetValue(out FlowPointState curCondition);

            BlueprintExecutePort nextPort;
            BlueprintExecuteState executeState;
            switch (curCondition)
            {
                default:
                    nextPort = null;
                    executeState = BlueprintExecuteState.End;
                    break;
                case FlowPointState.Running:
                    //下一个节点走运行中，但是本节点并未结束
                    nextPort = RunningExePort;
                    executeState = BlueprintExecuteState.Wait;
                    break;
                case FlowPointState.Fail:
                    nextPort = FailExePort;
                    executeState = BlueprintExecuteState.End;
                    break;
                case FlowPointState.Success:
                    nextPort = SuccessExePort;
                    executeState = BlueprintExecuteState.End;
                    break;
            }

            executePortResult.NextExecute = nextPort;
            executePortResult.ExecuteState = executeState;
            return executePortResult;
        }

        private BlueprintExecutePortResult OnOutputExecuted(BlueprintExecutePort executePort)
        {
            executePort.GetNextExecute(out BlueprintExecutePort targetPort);
            executePortResult.NextExecute = targetPort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }

        private FlowPointState GetConditionValue(BlueprintValuePort<FlowPointState> conditionPort)
        {
            FlowPointState state = condition;
            if (conditionPort.GetPrePortFirst(out BlueprintValuePort<FlowPointState> targetPort))
            {
                targetPort.GetValue(out state);
            }

            return state;
        }
    }
}