namespace PartsKit
{
    public abstract class BlueprintInOutputExeNodeBase : BlueprintNode
    {
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort OutputExePort { get; private set; }

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, OnInputExecuted);
            OutputExePort = BlueprintPortUtility.CreateExecutePort("OutputExe", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, OnOutputExecuted);
            AddPort(InputExePort);
            AddPort(OutputExePort);
        }

        protected virtual BlueprintExecutePortResult OnInputExecuted(BlueprintExecutePort executePort)
        {
            return BlueprintPortCommon.OnToTargetPortEndExecuted(OutputExePort);
        }

        protected virtual BlueprintExecutePortResult OnOutputExecuted(BlueprintExecutePort executePort)
        {
            return BlueprintPortCommon.OnToNextPortEndExecuted(executePort);
        }
    }
}