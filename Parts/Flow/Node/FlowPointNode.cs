using System;

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
        private const string CreateName = "Point(wait for running)";
        public override string NodeName => CreateName;
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort RunningExePort { get; private set; }
        public BlueprintExecutePort FailExePort { get; private set; }
        public BlueprintExecutePort SuccessExePort { get; private set; }
        public BlueprintPort<FlowPointState> ConditionPort { get; private set; }

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input);
            RunningExePort = BlueprintPortUtility.CreateExecutePort("RunningExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
            FailExePort = BlueprintPortUtility.CreateExecutePort("FailExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
            SuccessExePort = BlueprintPortUtility.CreateExecutePort("SuccessExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
            ConditionPort = BlueprintPortUtility.CreateInOutputPort<FlowPointState>("Condition",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input);

            AddPort(InputExePort);
            AddPort(RunningExePort);
            AddPort(FailExePort);
            AddPort(SuccessExePort);
            AddPort(ConditionPort);
        }

        protected override void OnExecuted(IBlueprintPort port, out BlueprintExecutePort nextPort,
            out BlueprintExecuteState executeState)
        {
            if (port != InputExePort)
            {
                throw new ArgumentOutOfRangeException();
            }

            FlowPointState curCondition = ConditionPort.GetValue();

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
        }
    }
}