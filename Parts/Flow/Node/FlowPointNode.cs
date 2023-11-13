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

        protected override void OnExecuted(IBlueprintPort port, out BlueprintExecutePort nextPort)
        {
            FlowPointState curCondition = ConditionPort.GetValue();

            switch (curCondition)
            {
                default:
                case FlowPointState.Default:
                case FlowPointState.Running:
                    RunningExePort.Execute(); //执行一下运行连接的逻辑
                    nextPort = null; //本次主线运行结束
                    break;
                case FlowPointState.Fail:
                    nextPort = FailExePort; //下一个节点为失败节点
                    break;
                case FlowPointState.Success:
                    nextPort = SuccessExePort; //下一个节点为成功节点
                    break;
            }
        }
    }
}