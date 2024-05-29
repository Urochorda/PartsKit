using System;
using UnityEngine;

namespace PartsKit
{
    public class BlueprintSetParameterNode : BlueprintParameterNodeBase
    {
        public override string NodeName => Parameter == null ? "Null" : $"Set ({Parameter.ParameterName})";

        public BlueprintValuePort<object> ValuePort { get; private set; }
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort OutputExePort { get; private set; }

        private BlueprintExecutePortResult executePortResult;

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

            if (Parameter != null)
            {
                ValuePort.PortType = Parameter.ParameterType; //覆盖为参数的type
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
            Parameter.Value = value;
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