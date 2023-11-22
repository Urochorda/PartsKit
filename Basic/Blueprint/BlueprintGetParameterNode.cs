using UnityEngine;

namespace PartsKit
{
    public class BlueprintGetParameterNode : BlueprintNode
    {
        [field: SerializeField] public string ParameterGuid { get; private set; }

        public BlueprintValuePort<object> ValuePort { get; private set; }

        private IBlueprintParameter parameter;

        public override string NodeName => parameter == null ? "Null" : $"Get ({parameter.ParameterName})";

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

            ValuePort = BlueprintPortUtility.CreateValuePort<object>("Value",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, string.Empty, GetParameterValue);

            if (parameter != null)
            {
                ValuePort.PortType = parameter.ParameterType; //覆盖为参数的type
            }

            AddPort(ValuePort);
        }

        private object GetParameterValue(BlueprintValuePort<object> arg)
        {
            return parameter.Value;
        }
    }
}