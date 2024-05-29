using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintNodeCreate(Blueprint.CommonNodeGroup, "Sequence", CreateName, "If流程节点")]
    public class BlueprintIfNode : BlueprintNode
    {
        private const string CreateName = "If-Bool";
        public override string NodeName => CreateName;
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort TreeOutputExePort { get; private set; }
        public BlueprintExecutePort FalseOutputExePort { get; private set; }
        public BlueprintValuePort<bool> ConditionPort { get; private set; }
        [SerializeField] private bool condition = true;

        private BlueprintExecutePortResult executePortResult;

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, OnInputExecuted);
            TreeOutputExePort = BlueprintPortUtility.CreateExecutePort("TrueExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output,
                BlueprintPortCommon.OnToNextPortEndExecuted);
            FalseOutputExePort = BlueprintPortUtility.CreateExecutePort("FalseExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output,
                BlueprintPortCommon.OnToNextPortEndExecuted);
            ConditionPort = BlueprintPortUtility.CreateValuePort<bool>("Condition",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, nameof(condition),
                GetConditionValue);

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

        private bool GetConditionValue(BlueprintValuePort<bool> conditionPort)
        {
            bool state = condition;
            if (conditionPort.GetPrePortValue(out bool prePortValue))
            {
                state = prePortValue;
            }

            return state;
        }
    }
}