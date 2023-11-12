using System;

namespace PartsKit
{
    [Serializable]
    [FlowCreateNode("Condition", CreateName, "If流程节点")]
    public class FlowIfNode : BlueprintNode
    {
        private const string CreateName = "If";
        public override string NodeName => CreateName;
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort TreeOutputExePort { get; private set; }
        public BlueprintExecutePort FalseOutputExePort { get; private set; }
        public BlueprintPort<bool> ConditionPort { get; private set; }

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input);
            TreeOutputExePort = BlueprintPortUtility.CreateExecutePort("TrueExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
            FalseOutputExePort = BlueprintPortUtility.CreateExecutePort("FalseExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
            ConditionPort = BlueprintPortUtility.CreateInOutputPort<bool>("Condition",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input);

            AddPort(InputExePort);
            AddPort(TreeOutputExePort);
            AddPort(FalseOutputExePort);
            AddPort(ConditionPort);
        }

        protected override BlueprintExecutePort OnExecuted(IBlueprintPort port)
        {
            bool isTree = ConditionPort.GetValue();
            return isTree ? TreeOutputExePort : FalseOutputExePort;
        }
    }
}