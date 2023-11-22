using System;
using UnityEngine;

namespace PartsKit
{
    public class BlueprintSetParameterNode : BlueprintNode
    {
        [field: SerializeField] public string ParameterGuid { get; private set; }

        public override string NodeName => parameter == null ? "Null" : $"Set ({parameter.ParameterName})";

        public BlueprintValuePort<object> ValuePort { get; private set; }
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort OutputExePort { get; private set; }

        private IBlueprintParameter parameter;
        private BlueprintExecutePortResult executePortResult;

        public void OnCreateParameterNode(string pGuid)
        {
            ParameterGuid = pGuid;
        }

        public override void Init(Blueprint blueprintVal)
        {
            parameter = blueprintVal.Blackboard.GetParameterByGuid(ParameterGuid);
            base.Init(blueprintVal);
        }

        public override bool IsNotValid()
        {
            return OwnerBlueprint == null || OwnerBlueprint.Blackboard.GetParameterByGuid(ParameterGuid) == null;
        }

        protected override void RegisterPort()
        {
            base.RegisterPort();

            GetSetValueInfo(out _, out string propertyFieldName);
            ValuePort = BlueprintPortUtility.CreateValuePort<object>("Value",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, propertyFieldName, GetParameterValue);

            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, OnInputExecuted);
            OutputExePort = BlueprintPortUtility.CreateExecutePort("OutputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);

            if (parameter != null)
            {
                ValuePort.PortType = parameter.ParameterType; //覆盖为参数的type
            }

            AddPort(InputExePort);
            AddPort(OutputExePort);
            AddPort(ValuePort);
        }

        private object GetParameterValue(BlueprintValuePort<object> arg)
        {
            GetSetValueInfo(out object infoVal, out _);
            if (arg.GetPrePortValue(out object prePortVal))
            {
                infoVal = prePortVal;
            }

            return infoVal;
        }

        private BlueprintExecutePortResult OnInputExecuted(BlueprintExecutePort executePort)
        {
            ValuePort.GetValue(out object value);
            parameter.Value = value;
            executePortResult.NextExecute = OutputExePort;
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

        protected virtual void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = default;
            propertyFieldNameVal = String.Empty;
        }
    }
}