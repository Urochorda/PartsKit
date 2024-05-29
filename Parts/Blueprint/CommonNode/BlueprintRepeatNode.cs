using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintNodeCreate(Blueprint.CommonNodeGroup, "Sequence", CreateName, "流程节点")]
    public class BlueprintRepeatNode : BlueprintInOutputExeNodeBase
    {
        private const string CreateName = "Repeat (Wait)";
        public override string NodeName => CreateName;

        public BlueprintValuePort<int> CountPort { get; private set; }
        [SerializeField] private int countDefaultValue = -1;

        private int curCount = 0;
        private BlueprintExecutePortResult inputExecutePortResult;

        protected override void RegisterPort()
        {
            base.RegisterPort();
            CountPort = BlueprintPortUtility.CreateValuePort<int>("Count",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(countDefaultValue), GetCountValue);

            AddPort(CountPort);
        }

        protected override BlueprintExecutePortResult OnInputExecuted(BlueprintExecutePort executePort)
        {
            CountPort.GetValue(out int countVal);
            //无限循环||次数未达到
            if (countVal < 0 || curCount < countVal)
            {
                inputExecutePortResult.ExecuteState = BlueprintExecuteState.Wait;
                curCount++;
            }
            else //次数到达了，最后一次
            {
                inputExecutePortResult.ExecuteState = BlueprintExecuteState.End;
                curCount = 0;
            }

            inputExecutePortResult.NextExecute = OutputExePort;
            return inputExecutePortResult;
        }

        private int GetCountValue(BlueprintValuePort<int> arg)
        {
            if (!arg.GetPrePortValue(out int countValue))
            {
                countValue = countDefaultValue;
            }

            return countValue;
        }
    }
}