﻿namespace PartsKit
{
    public class FlowRootNode : BlueprintNode
    {
        private const string CreateName = "Root";
        public override string NodeName => CreateName;
        public override bool Deletable => false;
        public BlueprintExecutePort OutputExePort { get; private set; }
        private BlueprintExecutePortResult executePortResult;

        protected override void RegisterPort()
        {
            base.RegisterPort();

            OutputExePort = BlueprintPortUtility.CreateExecutePort("OutputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);

            AddPort(OutputExePort);
        }

        private BlueprintExecutePortResult OnOutputExecuted(BlueprintExecutePort executePort)
        {
            executePort.GetNextExecute(out BlueprintExecutePort targetPort);
            executePortResult.NextExecute = targetPort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }
    }
}