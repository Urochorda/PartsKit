using System;

namespace PartsKit
{
    /// <summary>
    /// 流程节点状态
    /// </summary>
    public enum FlowPointState
    {
        Default = 0,
        Running = 1, //运行中
        Fail = 2, //失败
        Success = 3, //成功
    }

    [Serializable]
    [FlowCreateNode("Common", CreateName, "流程节点")]
    public class FlowPointNode : BlueprintNode
    {
        private const string CreateName = "Point";
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

        protected override BlueprintExecutePort OnExecuted(IBlueprintPort port)
        {
            FlowPointState curCondition = ConditionPort.GetValue();

            BlueprintExecutePort nextPort;
            switch (curCondition)
            {
                default:
                case FlowPointState.Default:
                case FlowPointState.Running:
                    nextPort = InputExePort; //继续执行
                    break;
                case FlowPointState.Fail:
                    nextPort = FailExePort; //失败节点
                    break;
                case FlowPointState.Success:
                    nextPort = SuccessExePort; //成功节点
                    break;
            }

            return nextPort;
        }
    }
}