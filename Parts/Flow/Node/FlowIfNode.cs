using System;
using UnityEngine;

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
        public BlueprintValuePort<bool> ConditionPort { get; private set; }
        [SerializeField] private bool Condition = true;

        private BlueprintExecutePortResult executePortResult;

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, OnInputExecuted);
            TreeOutputExePort = BlueprintPortUtility.CreateExecutePort("TrueExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);
            FalseOutputExePort = BlueprintPortUtility.CreateExecutePort("FalseExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);
            ConditionPort = BlueprintPortUtility.CreateValuePort<bool>("Condition",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, GetConditionValue);

            AddPort(InputExePort);
            AddPort(TreeOutputExePort);
            AddPort(FalseOutputExePort);
            AddPort(ConditionPort);
        }

        private BlueprintExecutePortResult OnInputExecuted(BlueprintExecutePort executePort)
        {
            ConditionPort.GetValue(out bool isTree);
            executePortResult.NextExecute = isTree ? TreeOutputExePort : FalseOutputExePort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }

        private BlueprintExecutePortResult OnOutputExecuted(BlueprintExecutePort executePort)
        {
            executePort.GetNextExecute(out BlueprintExecutePort targetPort);
            executePortResult.NextExecute = targetPort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }

        private bool GetConditionValue(BlueprintValuePort<bool> conditionPort)
        {
            bool state = Condition;
            if (conditionPort.GetPrePortFirst(out BlueprintValuePort<bool> targetPort))
            {
                targetPort.GetValue(out state);
            }

            return state;
        }
    }
}