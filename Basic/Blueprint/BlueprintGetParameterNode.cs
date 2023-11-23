namespace PartsKit
{
    public class BlueprintGetParameterNode : BlueprintParameterNodeBase
    {
        public BlueprintValuePort<object> ValuePort { get; private set; }

        public override string NodeName => Parameter == null ? "Null" : $"Get ({Parameter.ParameterName})";

        protected override void RegisterPort()
        {
            base.RegisterPort();

            ValuePort = BlueprintPortUtility.CreateValuePort<object>("Value",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, string.Empty, GetParameterValue);

            if (Parameter != null)
            {
                ValuePort.PortType = Parameter.ParameterType; //覆盖为参数的type
            }

            AddPort(ValuePort);
        }

        private object GetParameterValue(BlueprintValuePort<object> arg)
        {
            return Parameter.Value;
        }
    }
}