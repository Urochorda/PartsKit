namespace PartsKit
{
    public class FlowRootNode : BlueprintNode
    {
        private const string CreateName = "Root";
        public override string NodeName => CreateName;
        public override bool Deletable => false;

        public BlueprintExecutePort OutputExePort { get; private set; }

        protected override void RegisterPort()
        {
            base.RegisterPort();

            OutputExePort = BlueprintPortUtility.CreateExecutePort("OutputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);

            AddPort(OutputExePort);
        }
    }
}